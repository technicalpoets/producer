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

		const string usersDatabaseId = "Users";
		const string usersCollectionId = "Users";


		static Uri UsersCollectionLink = UriFactory.CreateDocumentCollectionUri (usersDatabaseId, usersCollectionId);


		public static async Task SaveUserStore (this DocumentClient client, string userId, string email, UserRoles role, TraceWriter log = null)
		{
			var userStore = new UserStore { Id = userId, Email = email?.ToLower (), UserRole = role };

			try
			{
				await client.CreateDocumentAsync (UsersCollectionLink, userStore);
			}
			catch (DocumentClientException dex)
			{
				dex.Print (log);

				switch (dex.StatusCode)
				{
					case HttpStatusCode.Conflict:

						log.Info ($"User document with id: {userId} already exists, replacing...");

						await client.ReplaceDocumentAsync (UriFactory.CreateDocumentUri (usersDatabaseId, usersCollectionId, userId), userStore);

						break;
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

				log?.Info ($"Reading User document with Id: {userId}");

				var response = await client.ReadDocumentAsync (UriFactory.CreateDocumentUri (usersDatabaseId, usersCollectionId, userId));

				var json = response?.Resource?.ToString ();

				if (!string.IsNullOrEmpty (json))
				{
					return JsonConvert.DeserializeObject<UserStore> (json);
				}

				return null;
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


		public static async Task<Permission> GetOrCreatePermission (this DocumentClient client, string dbId, string userId, string collectionId, string permissionId, PermissionMode permissionMode, TraceWriter log = null)
		{
			try
			{
				log?.Info ($"Attempting to read Document Collection with Id: {collectionId}");

				var collectionResponse = await client.ReadDocumentCollectionAsync (UriFactory.CreateDocumentCollectionUri (dbId, collectionId));

				var collection = collectionResponse?.Resource ?? throw new Exception ($"Could not find collection Database ID: {dbId}  Collection ID: {collectionId}");


				var userTup = await client.GetOrCreateUser (dbId, userId, log);

				var user = userTup.user;

				Permission permission;

				// if the user was newly created, go ahead and create the permission
				if (userTup.created && !string.IsNullOrEmpty (user?.SelfLink))
				{
					permission = await client.CreateNewPermission (collection, user, permissionId, permissionMode, log);
				}
				else // else look for an existing permission with the id
				{
					try
					{
						log?.Info ($"Attempting to read {permissionMode.ToString ()} permission with Id {permissionId} for user {userId}");

						var permissionResponse = await client.ReadPermissionAsync (UriFactory.CreatePermissionUri (dbId, userId, permissionId));

						permission = permissionResponse?.Resource;
					}
					catch (DocumentClientException dcx)
					{
						dcx.Print (log);

						switch (dcx.StatusCode)
						{
							case HttpStatusCode.NotFound:
								log?.Info ($"Did not find permission with Id {permissionId} for user: {userId} - creating...");

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
				log?.Error ($"Error creating PermissionToken for Database: {dbId}  Collection: {collectionId}  User: {userId}", ex);
				throw;
			}
		}


		public static async Task<Permission> CreateNewPermission (this DocumentClient client, DocumentCollection collection, User user, string permissionId, PermissionMode permissionMode, TraceWriter log = null)
		{
			try
			{
				log?.Info ($"Creating new {permissionMode} permission [Collection ID: {collection?.Id}  User ID: {user?.Id}  Permission ID: {permissionId}]");

				var newPermission = new Permission { Id = permissionId, ResourceLink = collection.SelfLink, PermissionMode = permissionMode };

				var permissionResponse = await client.CreatePermissionAsync (user.SelfLink, newPermission);

				return permissionResponse?.Resource;
			}
			catch (Exception ex)
			{
				log?.Error ($"Error creating new {permissionMode} permission [Collection ID: {collection?.Id}  User ID: {user?.Id}  Permission ID: {permissionId}]", ex);
				throw;
			}
		}


		public static async Task<(User user, bool created)> GetOrCreateUser (this DocumentClient client, string dbId, string userId, TraceWriter log = null)
		{
			User user = null;

			try
			{
				log?.Info ($"Attempting to read user with id: {userId}");

				var response = await client.ReadUserAsync (UriFactory.CreateUserUri (dbId, userId));

				user = response?.Resource;

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

						return (user, user != null);

					default: throw;
				}
			}
			catch (Exception ex)
			{
				log?.Error ($"Error getting user with id: {userId}\n", ex);
				throw;
			}
		}

	}
}
