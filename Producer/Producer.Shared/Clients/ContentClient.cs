using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

using Newtonsoft.Json;

using SettingsStudio;

using Producer.Domain;

namespace Producer.Shared
{
	public class ContentClient
	{
		static ContentClient _shared;
		public static ContentClient Shared => _shared ?? (_shared = new ContentClient ("Content"));


		readonly string databaseId;

		readonly DocumentClient client;


		public UserRoles UserRole { get; set; } = Settings.TestProducer ? UserRoles.Producer : UserRoles.General;

		public Dictionary<UserRoles, List<AvContent>> AvContent = new Dictionary<UserRoles, List<AvContent>> {
			{ UserRoles.General, new List<AvContent>() },
			{ UserRoles.Insider, new List<AvContent>() },
			{ UserRoles.Producer, new List<AvContent>() }
		};

		public event EventHandler<UserRoles> AvContentChanged;


		ContentClient (string dbId)
		{
			if (Settings.DocumentDbUrl == null)
			{
				throw new Exception ();
			}

			databaseId = dbId;

			Log.Debug ($"Creating DocumentClient\n\tUrl: {Settings.DocumentDbUrl}\n\tKey: {Settings.DocumentDbKey}");

			client = new DocumentClient (Settings.DocumentDbUrl, Settings.DocumentDbKey);
		}


		public async Task GetAllAvContent ()
		{
			await refreshAvContentAsync ();
		}


		public async Task<AvContent> CreateAvContent (AvContent item)
		{
			var newItem = await Create (item);

			AvContent [newItem.PublishedTo].Insert (0, newItem);

			AvContentChanged?.Invoke (this, newItem.PublishedTo);

			return newItem;
		}


		public async Task UpdateAvContent (AvContent item, UserRoles? role = null, bool publish = true)
		{
			if (role.HasValue && AvContent [role.Value].Remove (item))
			{
				// specify old role if role is what changed
				AvContentChanged?.Invoke (this, role.Value);
			}
			else if (AvContent [item.PublishedTo].Remove (item))
			{
				AvContentChanged?.Invoke (this, item.PublishedTo);
			}

			var newItem = await Replace (item);

			AvContent [newItem.PublishedTo].Add (newItem);

			//TODO: Sort
			//AvContent[newItem.PublishedTo].Sort(x, y)

			AvContentChanged?.Invoke (this, newItem.PublishedTo);

			if (publish)
			{
				await ProducerClient.Shared.Publish (item, role.HasValue ? item.DisplayName : null, role.HasValue ? "New Content!" : null);
			}
		}


		public async Task DeleteAvContent (AvContent item)
		{
			if (AvContent [item.PublishedTo].Remove (item))
			{
				AvContentChanged?.Invoke (this, item.PublishedTo);
			}

			var deletedItem = await Delete (item);
		}


		// public async Task PublishAvContent (AvContent item) {  }


		async Task refreshAvContentAsync ()
		{
			try
			{
				Expression<Func<AvContent, bool>> predicate = null;

				switch (UserRole)
				{
					case UserRoles.General:
						predicate = c => c.PublishedTo == UserRoles.General;
						break;
					case UserRoles.Insider:
						predicate = c => c.PublishedTo == UserRoles.General || c.PublishedTo == UserRoles.Insider;
						break;
					case UserRoles.Producer:
					case UserRoles.Admin:
						predicate = c => c.PublishedTo == UserRoles.General || c.PublishedTo == UserRoles.Insider || c.PublishedTo == UserRoles.Producer;
						break;
				}

				var content = await Get<AvContent> (predicate);
				//var content = new List<AvContent> ();
				AvContent = content.GroupBy (c => c.PublishedTo).ToDictionary (g => g.Key, g => g.OrderByDescending (i => i.Timestamp).ToList ());

				if (!AvContent.ContainsKey (UserRoles.General)) AvContent [UserRoles.General] = new List<AvContent> ();
				if (!AvContent.ContainsKey (UserRoles.Insider)) AvContent [UserRoles.Insider] = new List<AvContent> ();
				if (!AvContent.ContainsKey (UserRoles.Producer)) AvContent [UserRoles.Producer] = new List<AvContent> ();

				AvContentChanged?.Invoke (this, UserRole);
			}
			catch (Exception ex)
			{
				Log.Debug (ex.Message);
				throw;
			}
		}


		#region CRUD

		#region Get

		public async Task<T> Get<T> (string id, string collectionId = null)
			where T : Entity
		{
			if (string.IsNullOrEmpty (collectionId))
			{
				collectionId = typeof (T).Name;
			}

			try
			{
				if (!IsInitialized (collectionId))
				{
					await InitializeCollection (collectionId);
				}

				var result = await client.ReadDocumentAsync (UriFactory.CreateDocumentUri (databaseId, collectionId, id));

				return result.Resource as T;
			}
			catch (DocumentClientException dex)
			{
				dex.Print ();

				if (dex.StatusCode == System.Net.HttpStatusCode.NotFound)
				{
					return null;
				}
				else
				{
					throw;
				}
			}
			catch (Exception ex)
			{
				Log.Debug (ex.Message);
				throw;
			}
		}


		public async Task<List<T>> Get<T> (Expression<Func<T, bool>> predicate, string collectionId = null)
			where T : Entity
		{
			if (string.IsNullOrEmpty (collectionId))
			{
				collectionId = typeof (T).Name;
			}

			try
			{
				if (!IsInitialized (collectionId))
				{
					await InitializeCollection (collectionId);
				}

				var query = client.CreateDocumentQuery<T> (UriFactory.CreateDocumentCollectionUri (databaseId, collectionId), new FeedOptions { MaxItemCount = -1 }).Where (predicate).AsDocumentQuery ();

				var results = new List<T> ();

				while (query.HasMoreResults)
				{
					results.AddRange (await query.ExecuteNextAsync<T> ());
				}

				return results;
			}
			catch (DocumentClientException dex)
			{
				dex.Print ();

				if (dex.StatusCode == System.Net.HttpStatusCode.NotFound)
				{
					return null;
				}
				else
				{
					throw;
				}
			}
			catch (Exception ex)
			{
				Log.Debug (ex.Message);
				throw;
			}
		}

		#endregion


		public async Task<T> Create<T> (T item, string collectionId = null)
			where T : Entity
		{
			if (string.IsNullOrEmpty (collectionId))
			{
				collectionId = typeof (T).Name;
			}

			try
			{
				if (!IsInitialized (collectionId))
				{
					await InitializeCollection (collectionId);
				}

				var result = await client.CreateDocumentAsync (UriFactory.CreateDocumentCollectionUri (databaseId, collectionId), item);

				return JsonConvert.DeserializeObject<T> (result.Resource.ToString ());
			}
			catch (DocumentClientException dex)
			{
				dex.Print ();
				throw;
			}
			catch (Exception ex)
			{
				Log.Debug (ex.Message);
				throw;
			}
		}


		public async Task<T> Replace<T> (T item, string collectionId = null)
			where T : Entity
		{
			if (string.IsNullOrEmpty (collectionId))
			{
				collectionId = typeof (T).Name;
			}

			try
			{
				if (!IsInitialized (collectionId))
				{
					await InitializeCollection (collectionId);
				}

				var result = await client.ReplaceDocumentAsync (item.SelfLink, item);

				return JsonConvert.DeserializeObject<T> (result.Resource.ToString ());
			}
			catch (DocumentClientException dex)
			{
				dex.Print ();
				throw;
			}
			catch (Exception ex)
			{
				Log.Debug (ex.Message);
				throw;
			}
		}


		public async Task<T> Delete<T> (T item, string collectionId = null)
			where T : Entity
		{
			if (string.IsNullOrEmpty (collectionId))
			{
				collectionId = typeof (T).Name;
			}

			try
			{
				if (!IsInitialized (collectionId))
				{
					await InitializeCollection (collectionId);
				}

				var result = await client.DeleteDocumentAsync (item.SelfLink);

				if (!string.IsNullOrEmpty (result?.Resource?.ToString ()))
				{
					return JsonConvert.DeserializeObject<T> (result.Resource.ToString ());
				}

				return null;
			}
			catch (DocumentClientException dex)
			{
				dex.Print ();
				throw;
			}
			catch (Exception ex)
			{
				Log.Debug (ex.Message);
				throw;
			}
		}

		#endregion


		#region Initialization (database & collections)

		ClientStatus _databaseStatus;

		Task<ResourceResponse<Database>> _databaseTask;

		Dictionary<string, ClientStatus> _collectionStatuses = new Dictionary<string, ClientStatus> ();

		Dictionary<string, Task<ResourceResponse<DocumentCollection>>> _collectionCreationTasks = new Dictionary<string, Task<ResourceResponse<DocumentCollection>>> ();


		public bool IsInitialized (string collectionId) => _collectionStatuses.TryGetValue (collectionId, out ClientStatus status) && status == ClientStatus.Initialized;


		public async Task InitializeCollection (string collectionId)
		{
			if (_databaseStatus != ClientStatus.Initialized)
			{
				await createDatabaseIfNotExistsAsync ();
			}

			if (!(_collectionStatuses.TryGetValue (collectionId, out ClientStatus status) && status == ClientStatus.Initialized))
			{
				await createCollectionIfNotExistsAsync (collectionId);
			}
		}


		async Task createDatabaseIfNotExistsAsync ()
		{
			if (!_databaseTask.IsNullFinishCanceledOrFaulted ())
			{
				Log.Debug ("Database is already being created, returning existing task...");

				await _databaseTask;
			}
			else
			{
				try
				{
					Log.Debug ("Checking for Database...");

					_databaseTask = client.ReadDatabaseAsync (UriFactory.CreateDatabaseUri (databaseId));

					var database = await _databaseTask;

					if (database?.Resource != null)
					{
						_databaseStatus = ClientStatus.Initialized;

						Log.Debug ($"Found existing Database");
					}
				}
				catch (DocumentClientException dex)
				{
					if (dex.StatusCode == System.Net.HttpStatusCode.NotFound)
					{
						_databaseTask = client.CreateDatabaseAsync (new Database { Id = databaseId });

						var database = await _databaseTask;

						if (database?.Resource != null)
						{
							_databaseStatus = ClientStatus.Initialized;

							Log.Debug ($"Created new Database");
						}
					}
					else
					{
						Log.Debug (dex.Message);
						_databaseStatus = ClientStatus.NotInitialized;
						throw;
					}
				}
				catch (Exception ex)
				{
					Log.Debug (ex.Message);
					_databaseStatus = ClientStatus.NotInitialized;
					throw;
				}
			}
		}


		async Task createCollectionIfNotExistsAsync<T> ()
		{
			await createCollectionIfNotExistsAsync (typeof (T).Name);
		}


		async Task createCollectionIfNotExistsAsync (string collectionId)
		{
			if (_collectionCreationTasks.TryGetValue (collectionId, out Task<ResourceResponse<DocumentCollection>> task) && !task.IsNullFinishCanceledOrFaulted ())
			{
				Log.Debug ($"Collection: {collectionId} is already being created, returning existing task...");

				await task;
			}
			else
			{
				try
				{
					Log.Debug ($"Checking for Collection: {collectionId}...");

					_collectionCreationTasks [collectionId] = client.ReadDocumentCollectionAsync (UriFactory.CreateDocumentCollectionUri (databaseId, collectionId));

					var collection = await _collectionCreationTasks [collectionId];

					if (collection?.Resource != null)
					{
						_collectionStatuses [collectionId] = ClientStatus.Initialized;

						Log.Debug ($"Found existing Collection: {collectionId}");
					}
				}
				catch (DocumentClientException dex)
				{
					if (dex.StatusCode == System.Net.HttpStatusCode.NotFound)
					{
						_collectionCreationTasks [collectionId] = client.CreateDocumentCollectionAsync (UriFactory.CreateDatabaseUri (databaseId), new DocumentCollection { Id = collectionId }, new RequestOptions { OfferThroughput = 1000 });

						var collection = await _collectionCreationTasks [collectionId];

						if (collection?.Resource != null)
						{
							_collectionStatuses [collectionId] = ClientStatus.Initialized;
							Log.Debug ($"Created new Collection: {collectionId}");
						}
					}
					else
					{
						Log.Debug (dex.Message);
						_collectionStatuses [collectionId] = ClientStatus.NotInitialized;
						throw;
					}
				}
				catch (Exception ex)
				{
					Log.Debug (ex.Message);
					_collectionStatuses [collectionId] = ClientStatus.NotInitialized;
					throw;
				}
			}
		}

		#endregion
	}
}
