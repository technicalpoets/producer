using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

using Producer.Auth;
using Producer.Domain;

using HttpStatusCode = System.Net.HttpStatusCode;

namespace Producer.Functions
{
	public static class ContentTokenGenerator
	{

		const string contentDatabaseId = "Content";
		const string anonymousUserId = "anonymous_user";

		static readonly string _documentDbUri = Environment.GetEnvironmentVariable ("RemoteDocumentDbUrl");
		static readonly string _documentDbKey = Environment.GetEnvironmentVariable ("RemoteDocumentDbKey");

		static DocumentClient _documentClient;
		static DocumentClient DocumentClient => _documentClient ?? (_documentClient = new DocumentClient (new Uri ($"https://{_documentDbUri}/"), _documentDbKey));


		[Authorize]
		[FunctionName ("GetContentToken")]
		public static async Task<HttpResponseMessage> Run (
			[HttpTrigger (AuthorizationLevel.Anonymous, "get", Route = "tokens/content/{collectionId}")] HttpRequestMessage req, string collectionId, TraceWriter log)
		{
			try
			{
				var userId = Thread.CurrentPrincipal.GetClaimsIdentity ()?.UniqueIdentifier () ?? anonymousUserId;


				var userStore = await DocumentClient.GetUserStore (userId, log);

				// create anonymous UserStore if it doesn't alread exist
				if (userStore == null && userId == anonymousUserId)
				{
					userStore = await DocumentClient.SaveUserStore (anonymousUserId, anonymousUserId, UserRoles.General, log);
				}

				log.Info ($"Found User Store:\n{userStore?.ToString ()}");

				// if the token is still valid for the next 10 mins return it
				if (userStore?.ValidToken ?? false)
				{
					return req.CreateResponse (HttpStatusCode.OK, userStore.Token);
				}


				var permissionMode = userStore?.UserRole.CanWrite () ?? false ? PermissionMode.All : PermissionMode.Read;


				// simply getting the user permission will refresh the token
				var userPermission = await DocumentClient.GetOrCreatePermission (contentDatabaseId, userId, collectionId, permissionMode, log);


				if (!string.IsNullOrEmpty (userPermission?.Token))
				{
					var userStoreUpdate = await DocumentClient.UpdateUserStore (userStore, userPermission, log);

					log.Info ($"Updated User Store:\n{userStore?.ToString ()}");

					return req.CreateResponse (HttpStatusCode.OK, userStoreUpdate.Token);
				}


				return req.CreateResponse (HttpStatusCode.InternalServerError);

			}
			catch (Exception ex)
			{
				log.Error (ex.Message);

				return req.CreateErrorResponse (HttpStatusCode.InternalServerError, ex);
			}
		}
	}
}
