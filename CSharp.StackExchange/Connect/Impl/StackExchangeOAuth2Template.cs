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
using System.Collections.Specialized;
using Spring.Social.OAuth2;
using System.Threading.Tasks;

namespace CSharp.StackExchange.Connect.Impl
{
	/// <summary>
	/// StackExchange-specific extension of OAuth2Template that recognizes form-encoded responses for access token exchange.
	/// (The OAuth 2 specification indicates that an access token response should be in JSON format)
	/// </summary>
    public class StackExchangeOAuth2Template : OAuth2Template
    {
		/// <summary>
		/// Creates a new instance of <see cref="StackExchangeOAuth2Template"/>.
		/// </summary>
		/// <param name="clientId">The application's API client id.</param>
		/// <param name="clientSecret">The application's API client secret.</param>
        public StackExchangeOAuth2Template(string clientId, string clientSecret)
            : base(clientId, clientSecret,
				"https://stackexchange.com/oauth",
				"https://stackexchange.com/oauth/access_token")
        {
        }

        protected override Task<AccessGrant> PostForAccessGrantAsync(string accessTokenUrl, NameValueCollection request)
        {
            return RestTemplate.PostForObjectAsync<NameValueCollection>(accessTokenUrl, request)
                .ContinueWith<AccessGrant>(task =>
                {
                    var expires = task.Result["expires"];
                    return new AccessGrant(task.Result["access_token"], null, null, expires != null ? new int?(Int32.Parse(expires)) : null);
                });
        }

		protected AccessGrant PostForAccessGrant(string accessTokenUrl, NameValueCollection parameters)
		{
			var response = RestTemplate.PostForObject<NameValueCollection>(accessTokenUrl, parameters);
			var expires = response["expires"];
			return new AccessGrant(response["access_token"], null, null, expires != null ? new int?(Int32.Parse(expires)) : null);
		}
    }
}
