#region License

/*
 * Copyright 2002-2012 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion

using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using Spring.Http;
using Spring.Rest.Client;
using Spring.Rest.Client.Support;

namespace CSharp.StackExchange.Api.Impl
{
    /// <summary>
    /// Implementation of the <see cref="IResponseErrorHandler"/> that handles errors from StackExchange's REST API, 
    /// interpreting them into appropriate exceptions.
    /// </summary>
    /// <author>Scott Smith</author>
    class StackExchangeErrorHandler : DefaultResponseErrorHandler
    {
		// Default encoding for JSON
		private static readonly Encoding DefaultCharset = new UTF8Encoding(false);

    	/// <summary>
        /// Handles the error in the given response. 
        /// <para/>
        /// This method is only called when HasError() method has returned <see langword="true"/>.
        /// </summary>
        /// <remarks>
        /// This implementation throws appropriate exception if the response status code 
        /// is a client code error (4xx) or a server code error (5xx). 
        /// </remarks>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="requestMethod">The request method.</param>
        /// <param name="response">The response message with the error.</param>
        public override void HandleError(Uri requestUri, HttpMethod requestMethod, HttpResponseMessage<byte[]> response)
        {
			if (response == null) throw new ArgumentNullException("response");

            var type = (int)response.StatusCode / 100;
            switch (type)
            {
            	case 4:
            		HandleClientErrors(response);
            		break;
            	case 5:
            		HandleServerErrors(response.StatusCode);
            		break;
            }

            // if not otherwise handled, do default handling and wrap with StackExchangeApiException
            try
            {
                base.HandleError(requestUri, requestMethod, response);
            }
            catch (Exception ex)
            {
                throw new StackExchangeApiException("Error consuming StackExchange REST API.", ex);
            }
        }

		private void HandleClientErrors(HttpResponseMessage<byte[]> response)
		{
			if (response == null) throw new ArgumentNullException("response");

			var errorText = ExtractErrorDetailsFromResponse(response);

			throw new StackExchangeApiException(
				"The server indicated a client error has occured and returned the following HTTP status code: " + response.StatusCode +
				Environment.NewLine + Environment.NewLine +
				errorText,
				StackExchangeApiError.ClientError);
		}

    	private void HandleServerErrors(HttpStatusCode statusCode)
        {
			throw new StackExchangeApiException(
				"The server indicated a server error has occured and returned the following HTTP status code: " + statusCode,
				StackExchangeApiError.ServerError);
	    }

		private string ExtractErrorDetailsFromResponse(HttpResponseMessage<byte[]> response)
		{
			if (response.Body == null)
			{
				return null;
			}
			var contentType = response.Headers.ContentType;
			var charset = (contentType != null && contentType.CharSet != null) ? contentType.CharSet : DefaultCharset;

			var stream = new MemoryStream(response.Body);
			var gZipStream = new GZipStream(stream, CompressionMode.Decompress);

			var responseUncompressed = new byte[response.Body.Length * 10];
			var size = gZipStream.Read(responseUncompressed, 0, responseUncompressed.Length);

			return charset.GetString(responseUncompressed, 0, size);
		}
    }
}