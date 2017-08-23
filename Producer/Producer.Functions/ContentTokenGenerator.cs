using System;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

using HttpStatusCode = System.Net.HttpStatusCode;

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


		static readonly string _documentDbUri = Environment.GetEnvironmentVariable ("RemoteDocumentDbUrl");
		static readonly string _documentDbKey = Environment.GetEnvironmentVariable ("RemoteDocumentDbKey");

		static DocumentClient _docClient;
		public static DocumentClient DocClient => _docClient ?? (_docClient = new DocumentClient (new Uri ($"https://{_documentDbUri}/"), _documentDbKey));

		[Authorize]
		[FunctionName ("GetContentReadToken")]
		public static async Task<HttpResponseMessage> GetRead (
			[HttpTrigger (AuthorizationLevel.Anonymous, "get", Route = "tokens/read/{collectionId}")] HttpRequestMessage req, string collectionId, TraceWriter log)
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

						var userPermission = await getOrCreatePermission (databaseId, userId, collectionId, userReadId, PermissionMode.Read, log);

						if (!string.IsNullOrEmpty (userPermission?.Token))
						{
							return req.CreateResponse (HttpStatusCode.OK, userPermission.Token);
						}

						return req.CreateResponse (HttpStatusCode.InternalServerError);
					}
				}

				log.Info ("User is not authenticated, retrieving anonymous read token");

				var anonymousUserPermission = await getOrCreatePermission (databaseId, anonymousUserId, collectionId, anonymousReadId, PermissionMode.Read, log);

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


		[Authorize]
		[FunctionName ("GetContentWriteToken")]
		public static async Task<HttpResponseMessage> GetWrite (
			[HttpTrigger (AuthorizationLevel.Anonymous, "get", Route = "tokens/write/{collectionId}")] HttpRequestMessage req, string collectionId, TraceWriter log)
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
						var userPermission = await getOrCreatePermission (databaseId, userId, collectionId, userWriteId, PermissionMode.All, log);

						if (!string.IsNullOrEmpty (userPermission?.Token))
						{
							return req.CreateResponse (HttpStatusCode.OK, userPermission?.Token);
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

			log.Info ("User is not authenticated");

			return req.CreateResponse (HttpStatusCode.Unauthorized);
		}


		static async Task<Permission> getOrCreatePermission (string dbId, string userId, string collectionId, string permissionId, PermissionMode permissionMode, TraceWriter log = null)
		{
			try
			{
				var collectionResponse = await DocClient.ReadDocumentCollectionAsync (UriFactory.CreateDocumentCollectionUri (dbId, collectionId));

				var collection = collectionResponse?.Resource ?? throw new Exception ($"Could not find collection Database ID: {dbId}  Collection ID: {collectionId}");


				var userTup = await getOrCreateUser (dbId, userId, log);

				var user = userTup.user;

				Permission permission;

				// if the user was newly created, go ahead and create the permission
				if (userTup.created && !string.IsNullOrEmpty (user?.SelfLink))
				{
					permission = await createNewPermission (collection, user, permissionId, permissionMode, log);
				}
				else // else look for an existing permission with the id
				{
					try
					{
						log?.Info ($"Attempting to read permission with Id {permissionId} for user {userId}");

						var permissionResponse = await DocClient.ReadPermissionAsync (UriFactory.CreatePermissionUri (dbId, userId, permissionId));

						permission = permissionResponse?.Resource;
					}
					catch (DocumentClientException dcx)
					{
						if (dcx.StatusCode == HttpStatusCode.NotFound)
						{
							log?.Info ($"Did not find permission with Id {permissionId} for user: {userId} - creating...");

							permission = await createNewPermission (collection, user, permissionId, permissionMode, log);
						}
						else throw;
					}
				}

				return permission;
			}
			catch (Exception ex)
			{
				log?.Error ($"Error creating PermissionToken for Database: {dbId}  Collection: {collectionId}  User: {userId}", ex);
				throw;
			}
		}


		static async Task<Permission> createNewPermission (DocumentCollection collection, User user, string permissionId, PermissionMode permissionMode, TraceWriter log = null)
		{
			try
			{
				log?.Info ($"Creating new {permissionMode} permission [Collection ID: {collection?.Id}  User ID: {user?.Id}  Permission ID: {permissionId}]");

				var newPermission = new Permission { Id = permissionId, ResourceLink = collection.SelfLink, PermissionMode = permissionMode };

				var permissionResponse = await DocClient.CreatePermissionAsync (user.SelfLink, newPermission);

				return permissionResponse?.Resource;
			}
			catch (Exception ex)
			{
				log?.Error ($"Error creating new {permissionMode} permission [Collection ID: {collection?.Id}  User ID: {user?.Id}  Permission ID: {permissionId}]", ex);
				throw;
			}
		}


		static async Task<(User user, bool created)> getOrCreateUser (string dbId, string userId, TraceWriter log = null)
		{
			User user = null;

			try
			{
				log?.Info ($"Attempting to read user with id: {userId}");

				var response = await DocClient.ReadUserAsync (UriFactory.CreateUserUri (dbId, userId));

				user = response?.Resource;

				return (user, false);
			}
			catch (DocumentClientException dcx)
			{
				if (dcx.StatusCode == HttpStatusCode.NotFound)
				{
					log?.Info ($"Did not find user with Id {userId} - creating...");

					var response = await DocClient.CreateUserAsync (UriFactory.CreateDatabaseUri (dbId), new User { Id = userId });

					user = response?.Resource;

					return (user, user != null);
				}

				log?.Error ($"Error getting user with id: {userId}\n", dcx);
				throw;
			}
			catch (Exception ex)
			{
				log?.Error ($"Error getting user with id: {userId}\n", ex);
				throw;
			}
		}
	}
}
