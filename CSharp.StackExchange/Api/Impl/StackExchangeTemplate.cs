﻿#region License

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
using System.Collections.Generic;
using CSharp.StackExchange.Api.Interfaces;
using Spring.Json;
using Spring.Rest.Client;
using Spring.Http.Converters;
using Spring.Http.Converters.Json;
using Spring.Social.OAuth2;

namespace CSharp.StackExchange.Api.Impl
{
	/// <summary>
	/// This is the central class for interacting with StackExchange.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Some StackExchange operations require OAuth authentication. 
	/// To perform such operations, <see cref="StackExchangeTemplate"/> must be constructed 
	/// with the minimal amount of information required to sign requests to StackExchange's API 
	/// with an OAuth <code>Authorization</code> header.
	/// </para>
	/// <para>
	/// There are some operations that do not require OAuth authentication. 
	/// In those cases, you may use a <see cref="StackExchangeTemplate"/> that is created through 
	/// the default constructor and without any OAuth details.
	/// Attempts to perform secured operations through such an instance, however, 
	/// will result in <see cref="StackExchangeApiException"/> being thrown.
	/// </para>
	/// </remarks>
	/// <author>Scott Smith</author>
	public sealed class StackExchangeTemplate : AbstractOAuth2ApiBinding, IStackExchange 
    {
		private static readonly Uri ApiUriBase = new Uri("http://api.stackexchange.com/2.0/");

		/// <summary>
		/// Create a new instance of <see cref="StackExchangeTemplate"/> able to perform unauthenticated operations against StackExchange's API.
		/// </summary>
		/// <remarks>
		/// Some operations do not require OAuth authentication. 
		/// A StackExchangeTemplate created with this constructor will support those operations. 
		/// Any operations requiring authentication will throw an <see cref="StackExchangeApiException"/>.
		/// </remarks>
        public StackExchangeTemplate()
            : base()
        {
			InitSubApis();
        }

		/// <summary>
		/// Create a new instance of <see cref="StackExchangeTemplate"/>.
		/// </summary>
		/// <param name="accessToken">An access token acquired through OAuth authentication with StackExchange.</param>
        public StackExchangeTemplate(string accessToken)
            : base(accessToken)
        {
			InitSubApis();
        }

		#region IStackExchange Members

		/// <summary>
		/// Gets the underlying <see cref="IRestOperations"/> object allowing for consumption of StackExchange endpoints 
		/// that may not be otherwise covered by the API binding. 
		/// </summary>
		/// <remarks>
		/// The <see cref="IRestOperations"/> object returned is configured to include an OAuth "Authorization" header on all requests.
		/// </remarks>
		public IRestOperations RestOperations
		{
			get { return RestTemplate; }
		}

		#endregion

		/// <summary>
		/// Enables customization of the <see cref="RestTemplate"/> used to consume provider API resources.
		/// </summary>
		/// <remarks>
		/// An example use case might be to configure a custom error handler. 
		/// Note that this method is called after the RestTemplate has been configured with the message converters returned from GetMessageConverters().
		/// </remarks>
		/// <param name="restTemplate">The RestTemplate to configure.</param>
		protected override void ConfigureRestTemplate(RestTemplate restTemplate)
		{
			restTemplate.BaseAddress = ApiUriBase;
			restTemplate.ErrorHandler = new StackExchangeErrorHandler();
		}

		/// <summary>
		/// Returns a list of <see cref="IHttpMessageConverter"/>s to be used by the internal <see cref="RestTemplate"/>.
		/// </summary>
		/// <remarks>
		/// This implementation adds <see cref="SpringJsonHttpMessageConverter"/> and <see cref="ByteArrayHttpMessageConverter"/> to the default list.
		/// </remarks>
		/// <returns>
		/// The list of <see cref="IHttpMessageConverter"/>s to be used by the internal <see cref="RestTemplate"/>.
		/// </returns>
		protected override IList<IHttpMessageConverter> GetMessageConverters()
		{
			IList<IHttpMessageConverter> converters = base.GetMessageConverters();
			converters.Add(new ByteArrayHttpMessageConverter());
			converters.Add(GetJsonMessageConverter());
			return converters;
		}

		/// <summary>
		/// Returns a <see cref="SpringJsonHttpMessageConverter"/> to be used by the internal <see cref="RestTemplate"/>.
		/// <para/>
		/// Override to customize the message converter (for example, to set a custom object mapper or supported media types).
		/// </summary>
		/// <returns>The configured <see cref="SpringJsonHttpMessageConverter"/>.</returns>
		private SpringJsonHttpMessageConverter GetJsonMessageConverter()
		{
			var jsonMapper = new JsonMapper();

			return new SpringJsonHttpMessageConverter(jsonMapper);
		}

		private void InitSubApis()
		{
		}

        protected override OAuth2Version GetOAuth2Version()
        {
        	return OAuth2Version.Draft10;
        }
    }
}