using System;
using System.Linq;
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
using Producer.Domain;

namespace Producer.Functions
{
	public static class AuthUserConfigProvider
	{
		static HttpClient _httpClient;
		static HttpClient httpClient => _httpClient ?? (_httpClient = new HttpClient ());

		static readonly string [] _admins = Environment.GetEnvironmentVariable ("AppAdminEmails").ToLower ().Trim (';').Split (';');
		static readonly string [] _producers = Environment.GetEnvironmentVariable ("AppProducerEmails").ToLower ().Trim (';').Split (';');


		[Authorize]
		[FunctionName ("GetUserConfig")]
		public static async Task<HttpResponseMessage> Run (
			[HttpTrigger (AuthorizationLevel.Anonymous, "get", Route = "user/config")] HttpRequestMessage req, TraceWriter log)
		{
			var identity = Thread.CurrentPrincipal.GetClaimsIdentity ();

			if (identity != null)
			{
				var userId = identity.UniqueIdentifier ();

				log.Info ($"User is authenticated with userId: {userId}");

				httpClient.ConfigureClientForUserDetails (req);

				try
				{
					var me = await httpClient.GetStringAsync (new Uri (identity.UriFromIssuerClaim (), ".auth/me"));

					// TODO: Check for provider
					var googleUser = JsonConvert.DeserializeObject<GoogleAuthUser> (me.Trim (new Char [] { '[', ']' }));

					addRoleClaim (identity, googleUser);

					return req.CreateResponse (System.Net.HttpStatusCode.OK, googleUser.GetAuthUserConfig (userId));
				}
				catch (Exception ex)
				{
					log.Error ("Could not get user details", ex);
					throw;
				}
			}

			log.Info ("User is not authenticated");

			return req.CreateResponse (System.Net.HttpStatusCode.Unauthorized);
		}


		static void addRoleClaim (ClaimsIdentity identity, GoogleAuthUser googleUser)
		{
			var existingRole = identity.GetUserRole ();

			if (_admins.Contains (googleUser.EmailAddress.ToLower ()) && existingRole != UserRoles.Admin)
			{
				identity.AddClaim (new Claim (ClaimTypes.Role, UserRoles.Admin.Claim ()));
			}
			else if (_producers.Contains (googleUser.EmailAddress.ToLower ()) && existingRole != UserRoles.Admin && existingRole != UserRoles.Producer)
			{
				identity.AddClaim (new Claim (ClaimTypes.Role, UserRoles.Producer.Claim ()));
			}
			else if (existingRole != UserRoles.Admin && existingRole != UserRoles.Producer && existingRole != UserRoles.Insider)
			{
				identity.AddClaim (new Claim (ClaimTypes.Role, UserRoles.Insider.Claim ()));
			}
		}
	}
}