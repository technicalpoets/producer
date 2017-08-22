using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

using Producer.Auth;

namespace Producer.Functions
{
	public static class ContentTokenGenerator
	{
		const string databaseId = "Content";
		const string anonymousUserId = "anonymous_user";
		const string anonymousReadId = "anonymous_read";
		const string userWriteId = "user_write";
		const string userReadId = "user_read";


		[Authorize]
		[FunctionName ("GetContentReadToken")]
		public static async Task<HttpResponseMessage> GetRead (
			[HttpTrigger (AuthorizationLevel.Anonymous, "get", Route = "tokens/read/{collectionId}")] HttpRequestMessage req,
			[DocumentDB] DocumentClient client, string collectionId, TraceWriter log)
		{
			try
			{
				if (Thread.CurrentPrincipal.Identity.IsAuthenticated && Thread.CurrentPrincipal is ClaimsPrincipal principal)
				{
					log.Info ("User is authenticated");

					if (principal.Identity is ClaimsIdentity identity)
					{
						var userId = identity.UniqueIdentifier ();

						log.Info ($"userId = {userId}");

						var token = await getToken (client, userId, collectionId, userReadId, PermissionMode.Read);

						if (!string.IsNullOrEmpty (token))
						{
							return req.CreateResponse (System.Net.HttpStatusCode.OK, token);
						}

						return req.CreateResponse (System.Net.HttpStatusCode.InternalServerError);
					}
				}

				log.Info ("User is not authenticated, retrieving anonymous read token");

				var anonymousToken = await getToken (client, anonymousUserId, collectionId, anonymousReadId, PermissionMode.Read);

				if (!string.IsNullOrEmpty (anonymousToken))
				{
					return req.CreateResponse (System.Net.HttpStatusCode.OK, anonymousToken);
				}

				return req.CreateResponse (System.Net.HttpStatusCode.InternalServerError);
			}
			catch (Exception ex)
			{
				log.Error (ex.Message);

				return req.CreateErrorResponse (System.Net.HttpStatusCode.InternalServerError, ex);
			}
		}


		[Authorize]
		[FunctionName ("GetContentWriteToken")]
		public static async Task<HttpResponseMessage> GetWrite (
			[HttpTrigger (AuthorizationLevel.Anonymous, "get", Route = "tokens/write/{collectionId}")] HttpRequestMessage req,
			[DocumentDB] DocumentClient client, string collectionId, TraceWriter log)
		{
			if (Thread.CurrentPrincipal.Identity.IsAuthenticated && Thread.CurrentPrincipal is ClaimsPrincipal principal)
			{
				log.Info ("User is authenticated");

				if (principal.Identity is ClaimsIdentity identity)
				{
					var userId = identity.UniqueIdentifier ();

					log.Info ($"userId = {userId}");

					try
					{
						var token = await getToken (client, userId, collectionId, userWriteId, PermissionMode.All);

						if (!string.IsNullOrEmpty (token))
						{
							return req.CreateResponse (System.Net.HttpStatusCode.OK, token);
						}

						return req.CreateResponse (System.Net.HttpStatusCode.InternalServerError);
					}
					catch (Exception ex)
					{
						log.Error (ex.Message);

						return req.CreateErrorResponse (System.Net.HttpStatusCode.InternalServerError, ex);
					}
				}
			}

			log.Info ("User is not authenticated");

			return req.CreateResponse (System.Net.HttpStatusCode.Unauthorized);
		}


		static async Task<string> getToken (DocumentClient client, string userId, string collectionId, string permissionId, PermissionMode permissionMode)
		{
			var response = await client.ReadUserAsync (UriFactory.CreateUserUri (databaseId, userId));

			var user = response?.Resource;

			if (user == null)
			{
				response = await client.CreateUserAsync (UriFactory.CreateDatabaseUri (databaseId), new User { Id = userId });

				user = response?.Resource;

				if (!string.IsNullOrEmpty (user?.SelfLink))
				{
					var permResponse = await client.CreatePermissionAsync (user.SelfLink, new Permission { Id = permissionId, ResourceLink = UriFactory.CreateDocumentCollectionUri (databaseId, collectionId).LocalPath, PermissionMode = permissionMode });
				}
			}

			var permissions = new List<Permission> ();

			if (!string.IsNullOrEmpty (user?.PermissionsLink))
			{
				var readPermissions = await client.ReadPermissionFeedAsync (user.PermissionsLink);

				foreach (var perm in readPermissions)
				{
					permissions.Add (perm);
				}
			}

			var permission = permissions.FirstOrDefault ();

			return permission?.Token;
		}
	}
}
