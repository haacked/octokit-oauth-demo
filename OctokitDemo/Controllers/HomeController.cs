using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Octokit;
using OctokitDemo.Models;

namespace OctokitDemo.Controllers
{
    public class HomeController : Controller
    {
        const string clientId = "8b89b15af4ae1d408108";
        private const string clientSecret = "f2d3fdc7b3071bcdc9e9f99133884b15dc7c8d34";
        readonly GitHubClient client =
            new GitHubClient(new ProductHeaderValue("Haack-GitHub-Oauth-Demo"), new Uri("https://github.com/"));

        public async Task<ActionResult> Index(string code, string state)
        {
            if (!String.IsNullOrEmpty(code))
            {
                // TODO: Validate "state". This is like an anti-forgery token.
                var token = await client.Oauth.CreateAccessToken(
                    new OauthTokenRequest(clientId, clientSecret, code)
                    {
                        RedirectUri = new Uri("http://localhost:58292/") 
                    });
                Session["OAuthToken"] = token.AccessToken;
            }

            var accessToken = Session["OAuthToken"] as string;
            if (accessToken != null)
            {
                client.Credentials = new Credentials(accessToken);
            }

            try
            {
                var repositories = await client.Repository.GetAllForCurrent();
                var model = new IndexViewModel(repositories);

                return View(model);
            }
            catch (AuthorizationException)
            {
                // 1. Redirect users to request GitHub access
                var request = new OauthLoginRequest("8b89b15af4ae1d408108") {Scopes = {"user", "notifications"}};
                var oauthLoginUrl = client.Oauth.GetGitHubLoginUrl(request);
                return Redirect(oauthLoginUrl.ToString());
            }
        }

        public async Task<ActionResult> Emojis()
        {
            var emojis = await client.Miscellaneous.GetEmojis();

            return View(emojis);
        }
    }
}
