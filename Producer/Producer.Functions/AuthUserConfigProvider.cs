using System;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

using Newtonsoft.Json;

using Producer.Auth;

namespace Producer.Functions
{
	public static class AuthUserConfigProvider
	{
		static HttpClient _httpClient;
		static HttpClient httpClient => _httpClient ?? (_httpClient = new HttpClient ());

		[Authorize]
		[FunctionName ("GetUserConfig")]
		public static async Task<HttpResponseMessage> GetUserConfig (
			[HttpTrigger (AuthorizationLevel.Anonymous, "get", Route = "user/config")] HttpRequestMessage req, TraceWriter log)
		{
			if (Thread.CurrentPrincipal.Identity.IsAuthenticated && Thread.CurrentPrincipal is ClaimsPrincipal principal)
			{
				log.Info ("User is authenticated");

				if (principal.Identity is ClaimsIdentity identity)
				{
					var userId = identity.UniqueIdentifier ();

					log.Info ($"userId = {userId}");

					httpClient.ConfigureClientForUserDetails (identity, req);

					try
					{
						var me = await httpClient.GetStringAsync (".auth/me");

						// TODO: Check for provider
						var googleUser = JsonConvert.DeserializeObject<GoogleAuthUser> (me.Trim (new Char [] { '[', ']' }));

						return req.CreateResponse (System.Net.HttpStatusCode.OK, googleUser.GetAuthUserConfig (userId));
					}
					catch (Exception ex)
					{
						log.Error ("Could not get user details", ex);
						throw;
					}
				}
			}

			log.Info ("User is not authenticated");

			return req.CreateResponse (System.Net.HttpStatusCode.Unauthorized);
		}
	}
}