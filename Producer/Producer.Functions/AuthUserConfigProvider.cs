using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

using Microsoft.Azure.Documents.Client;
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

		static readonly string _documentDbUri = Environment.GetEnvironmentVariable ("RemoteDocumentDbUrl");
		static readonly string _documentDbKey = Environment.GetEnvironmentVariable ("RemoteDocumentDbKey");

		static readonly string [] _admins = Environment.GetEnvironmentVariable ("AppAdminEmails").ToLower ().Trim (';').Split (';');
		static readonly string [] _producers = Environment.GetEnvironmentVariable ("AppProducerEmails").ToLower ().Trim (';').Split (';');

		static HttpClient _httpClient;
		static HttpClient HttpClient => _httpClient ?? (_httpClient = new HttpClient ());

		static DocumentClient _documentClient;
		static DocumentClient DocumentClient => _documentClient ?? (_documentClient = new DocumentClient (new Uri ($"https://{_documentDbUri}/"), _documentDbKey));


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

				HttpClient.ConfigureClientForUserDetails (req);

				try
				{
					var me = await HttpClient.GetStringAsync (new Uri (identity.UriFromIssuerClaim (), ".auth/me"));

					// TODO: Check for provider
					var googleUser = JsonConvert.DeserializeObject<GoogleAuthUser> (me.Trim (new Char [] { '[', ']' }));


					var role = GetAuthorizedUserRole (googleUser);


					await DocumentClient.SaveUserStore (userId, googleUser?.EmailAddress, role, log);


					return req.CreateResponse (System.Net.HttpStatusCode.OK, googleUser.GetAuthUserConfig (userId, role));
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


		static UserRoles GetAuthorizedUserRole (GoogleAuthUser googleUser)
		{
			if (_admins.Contains (googleUser.EmailAddress.ToLower ()))
			{
				return UserRoles.Admin;
			}

			if (_producers.Contains (googleUser.EmailAddress.ToLower ()))
			{
				return UserRoles.Producer;
			}

			return UserRoles.Insider;
		}
	}
}