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
	public static class GenerateContentToken
	{
		static DocumentClient _documentClient;
		static DocumentClient DocumentClient => _documentClient ?? (_documentClient = new DocumentClient (EnvironmentVariables.DocumentDbUri, EnvironmentVariables.DocumentDbKey));


		[Authorize]
		[FunctionName (nameof (GenerateContentToken))]
		public static async Task<HttpResponseMessage> Run (
			[HttpTrigger (AuthorizationLevel.Anonymous, Routes.Get, Route = Routes.GenerateContentToken)] HttpRequestMessage req, string collectionId, TraceWriter log)
		{
			try
			{
				var userId = Thread.CurrentPrincipal.GetClaimsIdentity ()?.UniqueIdentifier () ?? UserStore.AnonymousId;


				var userStore = await DocumentClient.GetUserStore (userId, log);

				// create anonymous UserStore if it doesn't alread exist
				if (userStore == null && userId == UserStore.AnonymousId)
				{
					userStore = await DocumentClient.SaveUserStore (UserStore.AnonymousId, UserStore.AnonymousId, UserRoles.General, log);
				}

				log.Info ($"Found User Store:\n{userStore}");

				// if the token is still valid for the next 10 mins return it
				if (userStore?.ValidToken ?? false)
				{
					return req.CreateResponse (HttpStatusCode.OK, userStore.Token);
				}


				var permissionMode = userStore?.UserRole.CanWrite () ?? false ? PermissionMode.All : PermissionMode.Read;


				// simply getting the user permission will refresh the token
				var userPermission = await DocumentClient.GetOrCreatePermission ((nameof (Content), collectionId), userId, permissionMode, log);


				if (!string.IsNullOrEmpty (userPermission?.Token))
				{
					var userStoreUpdate = await DocumentClient.UpdateUserStore (userStore, userPermission, log);

					log.Info ($"Updated User Store:\n{userStore}");

					return req.CreateResponse (HttpStatusCode.OK, userStoreUpdate.Token);
				}


				return req.CreateResponse (HttpStatusCode.InternalServerError);
			}
			catch (Exception ex)
			{
				log.Error (ex.Message, ex);

				return req.CreateErrorResponse (HttpStatusCode.InternalServerError, ex);
			}
		}
	}
}
