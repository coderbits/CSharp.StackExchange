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
using System.Net;
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
            		HandleClientErrors(response.StatusCode);
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

		private void HandleClientErrors(HttpStatusCode statusCode)
        {
			if (statusCode == (HttpStatusCode)400)
			{
				throw new StackExchangeApiException(
					"An invalid parameter was passed, this includes even 'high level' parameters like key or site.",
					StackExchangeApiError.BadParameter);
			}

			if (statusCode == (HttpStatusCode)401)
        	{
        		throw new StackExchangeApiException(
					"A method that requires an access token (obtained via authentication) was called without one.",
        			StackExchangeApiError.AccessTokenRequired);
        	}

			if (statusCode == (HttpStatusCode)402)
			{
				throw new StackExchangeApiException(
					"An invalid access token was passed to a method.",
					StackExchangeApiError.InvalidAccessToken);
			}

			if (statusCode == (HttpStatusCode)403)
        	{
        		throw new StackExchangeApiException(
					"A method which requires certain permissions was called with an access token that lacks those permissions.",
        			StackExchangeApiError.AccessDenied);
        	}

			if (statusCode == (HttpStatusCode)404)
			{
				throw new StackExchangeApiException(
					"An attempt was made to call a method that does not exist. Note, calling methods that expect numeric ids (like /users/{ids}) with non-numeric ids can also result in this error.",
					StackExchangeApiError.NoMethod);
			}

			if (statusCode == (HttpStatusCode)405)
			{
				throw new StackExchangeApiException(
					"A method was called in a manner that requires an application key (generally, with an access token), but no key was passed.",
					StackExchangeApiError.KeyRequired);
			}

			if (statusCode == (HttpStatusCode)406)
			{
				throw new StackExchangeApiException(
					"An access token is no longer believed to be secure, normally because it was used on a non-HTTPS call. The access token will be invalidated if this error is returned.",
					StackExchangeApiError.AccessTokenCompromised);
			}
        }

    	private void HandleServerErrors(HttpStatusCode statusCode)
        {
			if (statusCode == (HttpStatusCode)500) 
            {
                throw new StackExchangeApiException(
					"An unexpected error occurred in the API. It has been logged, and Stack Exchange developers have been notified. You should report these errors on Stack Apps if you want to be notified when they're fixed.", 
                    StackExchangeApiError.InternalError);
		    }

			if (statusCode == (HttpStatusCode)502)
			{
				throw new StackExchangeApiException(
					"An application has violated part of the rate limiting contract, so the request was terminated.",
					StackExchangeApiError.ThrottleViolation);
			}

			if (statusCode == (HttpStatusCode)503)
			{
				throw new StackExchangeApiException(
					"Some or all of the API is unavailable. Applications should backoff on requests to the method invoked.",
					StackExchangeApiError.TemporarilyUnavailable);
			}
	    }
    }
}