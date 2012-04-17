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
using CSharp.StackExchange.Api.Impl;
using CSharp.StackExchange.Api.Interfaces;
using CSharp.StackExchange.Connect.Impl;
using Spring.Social.OAuth2;

namespace CSharp.StackExchange.Connect
{
	/// <summary>
	/// StackExchange <see cref="IServiceProvider"/> implementation.
	/// </summary>
	/// <author>Scott Smith</author>
    public class StackExchangeServiceProvider : AbstractOAuth2ServiceProvider<IStackExchange>
    {
		/// <summary>
		/// Creates a new instance of <see cref="StackExchangeServiceProvider"/>.
		/// </summary>
		/// <param name="clientId">The application's API client id.</param>
		/// <param name="clientSecret">The application's API client secret.</param>
        public StackExchangeServiceProvider(String clientId, String clientSecret)
			: base(new StackExchangeOAuth2Template(clientId, clientSecret))
        {
        }

		/// <summary>
		/// Returns an API interface allowing the client application to access unprotected resources.
		/// </summary>
		/// <returns>A binding to the service provider's API.</returns>
		public IStackExchange GetAPi()
		{
			return new StackExchangeTemplate();
		}

		/// <summary>
		/// Returns an API interface allowing the client application to access protected resources on behalf of a user.
		/// </summary>
		/// <param name="accessToken">The API access token.</param>
		/// <returns>A binding to the service provider's API.</returns>
		public override IStackExchange GetApi(string accessToken)
		{
			return new StackExchangeTemplate(accessToken);
		}
    }
}
