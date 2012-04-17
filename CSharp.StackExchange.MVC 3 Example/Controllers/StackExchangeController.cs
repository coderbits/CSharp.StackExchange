using System.Web.Mvc;
using CSharp.StackExchange.Api.Interfaces;
using CSharp.StackExchange.Connect;
using Spring.Social.OAuth2;

namespace CSharp.StackExchange.MVC_3_Example.Controllers
{
	public class StackExchangeController : Controller
	{
		// Register your own StackExchange app at https://api.stackexchange.com/docs

		// Configure the Callback URL
		private const string CallbackUrl = "http://localhost/StackExchange/Callback";

		// Set your consumer key & secret here
		private const string StackExchangeApiClientId = "ENTER YOUR CLIENT ID HERE";
		private const string StackExchangeApiSecret = "ENTER YOUR SECRET HERE";

		readonly IOAuth2ServiceProvider<IStackExchange> _stackExchangeProvider = new StackExchangeServiceProvider(StackExchangeApiClientId, StackExchangeApiSecret);

		public ActionResult Index()
		{
			var accessGrant = Session["AccessGrant"] as AccessGrant;
			if (accessGrant != null)
			{
				ViewBag.AccessToken = accessGrant.AccessToken;

				return View();
			}

			var parameters = new OAuth2Parameters
			{
				RedirectUrl = CallbackUrl,
				Scope = "no_expiry"
			};
			return Redirect(_stackExchangeProvider.OAuthOperations.BuildAuthorizeUrl(GrantType.AuthorizationCode, parameters));
		}

		public ActionResult Callback(string code)
		{
			AccessGrant accessGrant = _stackExchangeProvider.OAuthOperations.ExchangeForAccessAsync(code, CallbackUrl, null).Result;

			Session["AccessGrant"] = accessGrant;

			return RedirectToAction("Index");
		}
	}
}
