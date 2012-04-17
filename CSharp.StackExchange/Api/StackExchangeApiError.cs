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

namespace CSharp.StackExchange.Api
{
    /// <summary>
    /// The <see cref="StackExchangeApiError"/> enumeration is used by the <see cref="StackExchangeApiException"/> class 
    /// to indicate what kind of error caused the exception.
    /// </summary>
    /// <author>Scott Smith</author>
    public enum StackExchangeApiError
    {
		/// <summary>
		/// 400 status code. An invalid parameter was passed, this includes even "high level" parameters like key or site.
		/// </summary>
		BadParameter,

        /// <summary>
		/// 401 status code. A method that requires an access token (obtained via authentication) was called without one.
        /// </summary>
        AccessTokenRequired,

		/// <summary>
		/// 402 status code. An invalid access token was passed to a method.
		/// </summary>
		InvalidAccessToken,

		/// <summary>
		/// 403 status code. A method which requires certain permissions was called with an access token that lacks those permissions.
		/// </summary>
		AccessDenied,

        /// <summary>
		/// 404 status code. An attempt was made to call a method that does not exist. Note, calling methods that expect numeric ids (like /users/{ids}) with non-numeric ids can also result in this error.
        /// </summary>
        NoMethod,

		/// <summary>
		/// 405 status code. A method was called in a manner that requires an application key (generally, with an access token), but no key was passed.
		/// </summary>
		KeyRequired,

		/// <summary>
		/// 406 status code. An access token is no longer believed to be secure, normally because it was used on a non-HTTPS call. The access token will be invalidated if this error is returned.
		/// </summary>
		AccessTokenCompromised,

        /// <summary>
		/// 500 status code. An unexpected error occurred in the API. It has been logged, and Stack Exchange developers have been notified. You should report these errors on Stack Apps if you want to be notified when they're fixed.
        /// </summary>
        InternalError,

		/// <summary>
		/// 502 status code. An application has violated part of the rate limiting contract, so the request was terminated.
		/// </summary>
		ThrottleViolation,

		/// <summary>
		/// 503 status code. Some or all of the API is unavailable. Applications should backoff on requests to the method invoked.
		/// </summary>
		TemporarilyUnavailable
    }
}
