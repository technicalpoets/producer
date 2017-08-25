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
				var userId = Thread.CurrentPrincipal.GetClaimsIdentity ()?.UniqueIdentifier ();

				if (!string.IsNullOrEmpty (userId))
				{
					log.Info ($"User is authenticated and has Id: {userId}");

					var userStore = await DocumentClient.GetUserStore (userId, log);


					var permissionMode = userStore?.UserRole.CanWrite () ?? false ? PermissionMode.All : PermissionMode.Read;


					var userPermission = await DocumentClient.GetOrCreatePermission (contentDatabaseId, userId, collectionId, permissionMode, log);

					if (!string.IsNullOrEmpty (userPermission?.Token))
					{
						return req.CreateResponse (HttpStatusCode.OK, userPermission.Token);
					}

					return req.CreateResponse (HttpStatusCode.InternalServerError);
				}

				log.Info ("User is not authenticated, retrieving anonymous read token");

				var anonymousUserPermission = await DocumentClient.GetOrCreatePermission (contentDatabaseId, anonymousUserId, collectionId, PermissionMode.Read, log);

				if (!string.IsNullOrEmpty (anonymousUserPermission?.Token))
				{
					return req.CreateResponse (HttpStatusCode.OK, anonymousUserPermission.Token);
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
