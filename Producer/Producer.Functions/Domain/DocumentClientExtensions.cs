using System;
using System.Threading.Tasks;

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs.Host;

using Newtonsoft.Json;

using Producer.Domain;

using User = Microsoft.Azure.Documents.User;
using HttpStatusCode = System.Net.HttpStatusCode;

namespace Producer.Functions
{
	public static class DocumentClientExtensions
	{

		static Uri UsersCollectionLink = UriFactory.CreateDocumentCollectionUri (UserStore.DatabaseId, UserStore.CollectionId);


		static RequestOptions permissionRequestOptions = new RequestOptions { ResourceTokenExpirySeconds = UserStore.TokenDurationSeconds };


		static string GetUserPermissionId (string dbId, string userId, PermissionMode permissionMode) => $"{dbId}-{userId}-{permissionMode.ToString ().ToUpper ()}";


		public static async Task<UserStore> SaveUserStore (this DocumentClient client, string userId, string email, UserRoles role, TraceWriter log = null)
		{
			var userStore = new UserStore { Id = userId, Email = email?.ToLower (), UserRole = role };

			try
			{
				log?.Info ($"Attempting to create new UserStore document with Id: {userId}");

				var response = await client.CreateDocumentAsync (UsersCollectionLink, userStore);

				var json = response?.Resource?.ToString ();

				return string.IsNullOrEmpty (json) ? null : JsonConvert.DeserializeObject<UserStore> (json);
			}
			catch (DocumentClientException dex)
			{
				dex.Print (log);

				switch (dex.StatusCode)
				{
					case HttpStatusCode.Conflict:

						log.Info ($"UserStore document with id: {userId} already exists, replacing...");

						var response = await client.ReplaceDocumentAsync (UriFactory.CreateDocumentUri (UserStore.DatabaseId, UserStore.CollectionId, userId), userStore);

						var json = response?.Resource?.ToString ();

						return string.IsNullOrEmpty (json) ? null : JsonConvert.DeserializeObject<UserStore> (json);

					default: throw;
				}
			}
			catch (Exception ex)
			{
				log?.Error ("Error saving new User", ex);
				throw;
			}
		}


		public static async Task<UserStore> GetUserStore (this DocumentClient client, string userId, TraceWriter log = null)
		{
			try
			{
				if (string.IsNullOrEmpty (userId))
				{
					return null;
				}

				log?.Info ($"Attempting to get UserStore document with Id: {userId}");

				var response = await client.ReadDocumentAsync (UriFactory.CreateDocumentUri (UserStore.DatabaseId, UserStore.CollectionId, userId));

				var json = response?.Resource?.ToString ();

				return string.IsNullOrEmpty (json) ? null : JsonConvert.DeserializeObject<UserStore> (json);
			}
			catch (DocumentClientException dex)
			{
				dex.Print (log);

				switch (dex.StatusCode)
				{
					case HttpStatusCode.NotFound: return null;
					default: throw;
				}
			}
			catch (Exception ex)
			{
				log?.Error ("Error saving new User", ex);
				throw;
			}
		}


		public static async Task<UserStore> UpdateUserStore (this DocumentClient client, UserStore userStore, Permission permission, TraceWriter log = null)
		{
			try
			{
				if (string.IsNullOrEmpty (userStore?.Id) || string.IsNullOrEmpty (permission?.Token))
				{
					return null;
				}

				log?.Info ($"Attempting to replace UserStore document with Id: {userStore.Id}");

				userStore.Token = permission.Token;
				userStore.TokenTimestamp = DateTime.UtcNow;


				var response = await client.ReplaceDocumentAsync (userStore.SelfLink, userStore);

				var json = response?.Resource?.ToString ();

				return string.IsNullOrEmpty (json) ? null : JsonConvert.DeserializeObject<UserStore> (json);
			}
			catch (DocumentClientException dex)
			{
				dex.Print (log);

				switch (dex.StatusCode)
				{
					case HttpStatusCode.NotFound:

						var response = await client.CreateDocumentAsync (UriFactory.CreateDocumentCollectionUri (UserStore.DatabaseId, UserStore.CollectionId), userStore);

						var json = response?.Resource?.ToString ();

						return string.IsNullOrEmpty (json) ? null : JsonConvert.DeserializeObject<UserStore> (json);

					default: throw;
				}
			}
			catch (Exception ex)
			{
				log?.Error ("Error saving new User", ex);
				throw;
			}
		}


		public static async Task<Permission> GetOrCreatePermission (this DocumentClient client, string dbId, string userId, string collectionId, PermissionMode permissionMode, TraceWriter log = null)
		{
			var permissionId = string.Empty;

			try
			{
				log?.Info ($"Attempting to get Document Collection with Id: {collectionId}");

				var collectionResponse = await client.ReadDocumentCollectionAsync (UriFactory.CreateDocumentCollectionUri (dbId, collectionId));

				var collection = collectionResponse?.Resource ?? throw new Exception ($"Could not find Document Collection in Database {dbId} with CollectionId: {collectionId}");


				var userTup = await client.GetOrCreateUser (dbId, userId, log);

				var user = userTup.user;

				Permission permission;

				permissionId = GetUserPermissionId (dbId, user.Id, permissionMode);

				// if the user was newly created, go ahead and create the permission
				if (userTup.created && !string.IsNullOrEmpty (user?.Id))
				{
					permission = await client.CreateNewPermission (collection, user, permissionId, permissionMode, log);
				}
				else // else look for an existing permission with the id
				{
					var permissionUri = UriFactory.CreatePermissionUri (dbId, user.Id, permissionId);

					try
					{
						//int count = 0;

						//string continuation = string.Empty;

						//do
						//{
						//	// Read the feed 10 items at a time until there are no more items to read
						//	var r = await client.ReadPermissionFeedAsync (user.PermissionsLink, new FeedOptions { MaxItemCount = -1, RequestContinuation = continuation });

						//	// Append the item count
						//	count += r.Count;

						//	// Get the continuation so that we know when to stop.
						//	continuation = r.ResponseContinuation;

						//	foreach (var i in r)
						//	{
						//		log?.Info ($"permission.Id:             {i.Id}");
						//		log?.Info ($"permission.ResourceId:     {i.ResourceId}");
						//		log?.Info ($"permission.ResourceLink:   {i.ResourceLink}");
						//		log?.Info ($"permission.AltLink:        {i.AltLink}");
						//		log?.Info ($"permission.PermissionMode: {i.PermissionMode}");
						//		log?.Info ($"permission.SelfLink:       {i.SelfLink}");
						//		log?.Info ($"permission.Timestamp:      {i.Timestamp}");
						//		log?.Info ($"permission.Token:          {i.Token}");
						//		log?.Info ($"permission.ETag:           {i.ETag}");
						//		log?.Info ($"");
						//	}

						//} while (!string.IsNullOrEmpty (continuation));


						log?.Info ($"Attempting to get Permission with Id: {permissionId}  at Uri: {permissionUri}");

						var permissionResponse = await client.ReadPermissionAsync (permissionUri, permissionRequestOptions);

						permission = permissionResponse?.Resource;

						if (permission != null)
						{
							log?.Info ($"Found existing Permission with Id: {permission.Id}");
						}
					}
					catch (DocumentClientException dcx)
					{
						dcx.Print (log);

						switch (dcx.StatusCode)
						{
							case HttpStatusCode.NotFound:

								log?.Info ($"Did not find Permission with Id: {permissionId}  at Uri: {permissionUri} - creating...");

								permission = await client.CreateNewPermission (collection, user, permissionId, permissionMode, log);

								break;
							default: throw;
						}
					}
				}

				return permission;
			}
			catch (Exception ex)
			{
				log?.Error ($"Error creating new new {permissionMode.ToString ().ToUpper ()} Permission [Database: {dbId} Collection: {collectionId}  User: {userId}  Permission: {permissionId}", ex);
				throw;
			}
		}


		public static async Task<Permission> CreateNewPermission (this DocumentClient client, DocumentCollection collection, User user, string permissionId, PermissionMode permissionMode, TraceWriter log = null)
		{
			log?.Info ($"Creating new Permission with Id: {permissionId}  for Collection: {collection?.Id}");

			var newPermission = new Permission { Id = permissionId, ResourceLink = collection.SelfLink, PermissionMode = permissionMode };

			try
			{
				var permissionResponse = await client.CreatePermissionAsync (user.SelfLink, newPermission, permissionRequestOptions);

				var permission = permissionResponse?.Resource;

				if (permission != null)
				{
					log?.Info ($"Created new Permission with Id: {permission.Id}");
				}

				return permission;
			}
			catch (DocumentClientException dcx)
			{
				dcx.Print (log);

				switch (dcx.StatusCode)
				{
					case HttpStatusCode.Conflict:

						var oldPermissionId = permissionId.Replace (permissionMode.ToString ().ToUpper (), permissionMode == PermissionMode.All ? PermissionMode.Read.ToString ().ToUpper () : PermissionMode.All.ToString ().ToUpper ());

						log?.Info ($"Deleting old Permission with Id: {oldPermissionId}...");

						await client.DeletePermissionAsync (UriFactory.CreatePermissionUri ("Content", user.Id, oldPermissionId));

						log?.Info ($"Creating new Permission with Id: {permissionId}  for Collection: {collection?.Id}");

						var permissionResponse = await client.CreatePermissionAsync (user.SelfLink, newPermission, permissionRequestOptions);

						var permission = permissionResponse?.Resource;

						if (permission != null)
						{
							log?.Info ($"Created new Permission with Id: {permission.Id}");
						}

						return permission;

					default: throw;
				}
			}
			catch (Exception ex)
			{
				log?.Error ($"Error creating new Permission with Id: {permissionId}  for Collection: {collection?.Id}", ex);
				throw;
			}
		}


		public static async Task<(User user, bool created)> GetOrCreateUser (this DocumentClient client, string dbId, string userId, TraceWriter log = null)
		{
			User user = null;

			try
			{
				log?.Info ($"Attempting to get Database ({dbId}) User with Id: {userId}");

				var response = await client.ReadUserAsync (UriFactory.CreateUserUri (dbId, userId));

				user = response?.Resource;

				if (user != null)
				{
					log?.Info ($"Found existing Database ({dbId}) User with Id {userId}");
				}

				return (user, false);
			}
			catch (DocumentClientException dcx)
			{
				dcx.Print (log);

				switch (dcx.StatusCode)
				{
					case HttpStatusCode.NotFound:

						log?.Info ($"Did not find user with Id {userId} - creating...");

						var response = await client.CreateUserAsync (UriFactory.CreateDatabaseUri (dbId), new User { Id = userId });

						user = response?.Resource;

						if (user != null)
						{
							log?.Info ($"Created new Database ({dbId}) User with Id {userId}");
						}

						return (user, user != null);

					default: throw;
				}
			}
			catch (Exception ex)
			{
				log?.Error ($"Error getting User with Id: {userId}\n", ex);
				throw;
			}
		}


		public static string PermissionLink (this User user, string permissionId) => $"{user?.PermissionsLink}{permissionId}";
	}
}
