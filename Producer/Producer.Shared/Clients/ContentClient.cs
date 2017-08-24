using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using HttpStatusCode = System.Net.HttpStatusCode;

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

using Newtonsoft.Json;

using SettingsStudio;

using Producer.Domain;
using Producer.Auth;

namespace Producer.Shared
{
	public class ContentClient
	{
		static ContentClient _shared;
		public static ContentClient Shared => _shared ?? (_shared = new ContentClient ("Content"));


		readonly string databaseId;

		DocumentClient client;


		public UserRoles UserRole => ProducerClient.Shared.UserRole;

		public Dictionary<UserRoles, List<AvContent>> AvContent = new Dictionary<UserRoles, List<AvContent>> {
			{ UserRoles.General, new List<AvContent>() },
			{ UserRoles.Insider, new List<AvContent>() },
			{ UserRoles.Producer, new List<AvContent>() }
		};

		public event EventHandler<UserRoles> AvContentChanged;


		ContentClient (string dbId)
		{
			databaseId = dbId;
		}

		public void ResetClient ()
		{
			client = null;
		}

		async Task refreshResourceToken<T> (bool forceTokenRefresh = true)
			where T : Entity
		{
			try
			{
				var resourceToken = await ProducerClient.Shared.GetContentToken<T> (forceTokenRefresh);

				resetClient (resourceToken);
			}
			catch (FormatException)
			{
				var resourceToken = await ProducerClient.Shared.GetContentToken<T> (true);

				resetClient (resourceToken);
			}
		}


		void resetClient (string resourceToken)
		{
			Log.Debug ($"Creating DocumentClient\n\tUrl: {Settings.DocumentDbUrl}\n\tKey: {resourceToken}");

			try
			{
				client = new DocumentClient (Settings.DocumentDbUrl, resourceToken);
			}
			catch (Exception ex)
			{
				Log.Debug (ex.Message);
				throw;
			}
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
				if (client == null) await refreshResourceToken<T> (false);
				//if (!IsInitialized (collectionId)) await InitializeCollection (collectionId);

				updateNetworkActivityIndicator (true);

				var result = await client.ReadDocumentAsync (UriFactory.CreateDocumentUri (databaseId, collectionId, id));

				return result.Resource as T;
			}
			catch (DocumentClientException dex)
			{
				dex.Print ();

				switch (dex.StatusCode)
				{
					case HttpStatusCode.NotFound: return null;
					case HttpStatusCode.Forbidden:

						await refreshResourceToken<T> ();

						var result = await client.ReadDocumentAsync (UriFactory.CreateDocumentUri (databaseId, collectionId, id));

						return result.Resource as T;

					default: throw;
				}
			}
			catch (Exception ex)
			{
				Log.Debug (ex.Message);
				throw;
			}
			finally
			{
				updateNetworkActivityIndicator (false);
			}
		}


		public async Task<List<T>> Get<T> (Expression<Func<T, bool>> predicate, string collectionId = null)
			where T : Entity
		{
			if (string.IsNullOrEmpty (collectionId))
			{
				collectionId = typeof (T).Name;
			}

			var results = new List<T> ();

			try
			{
				if (client == null) await refreshResourceToken<T> (false);
				// if (!IsInitialized (collectionId)) await InitializeCollection (collectionId);

				updateNetworkActivityIndicator (true);

				var query = client.CreateDocumentQuery<T> (UriFactory.CreateDocumentCollectionUri (databaseId, collectionId), new FeedOptions { MaxItemCount = -1 }).Where (predicate).AsDocumentQuery ();

				while (query.HasMoreResults)
				{
					results.AddRange (await query.ExecuteNextAsync<T> ());
				}

				return results;
			}
			catch (DocumentClientException dex)
			{
				dex.Print ();

				switch (dex.StatusCode)
				{
					case HttpStatusCode.NotFound: return null;
					case HttpStatusCode.Forbidden:

						await refreshResourceToken<T> ();

						var query = client.CreateDocumentQuery<T> (UriFactory.CreateDocumentCollectionUri (databaseId, collectionId), new FeedOptions { MaxItemCount = -1 }).Where (predicate).AsDocumentQuery ();

						while (query.HasMoreResults)
						{
							results.AddRange (await query.ExecuteNextAsync<T> ());
						}

						return results;

					default: throw;
				}
			}
			catch (Exception ex)
			{
				Log.Debug (ex.Message);
				throw;
			}
			finally
			{
				updateNetworkActivityIndicator (false);
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
				if (client == null) await refreshResourceToken<T> (false);
				//if (!IsInitialized (collectionId)) await InitializeCollection (collectionId);

				updateNetworkActivityIndicator (true);

				var result = await client.CreateDocumentAsync (UriFactory.CreateDocumentCollectionUri (databaseId, collectionId), item);

				return JsonConvert.DeserializeObject<T> (result.Resource.ToString ());
			}
			catch (DocumentClientException dex)
			{
				dex.Print ();

				switch (dex.StatusCode)
				{
					case HttpStatusCode.Forbidden:

						await refreshResourceToken<T> ();

						var result = await client.CreateDocumentAsync (UriFactory.CreateDocumentCollectionUri (databaseId, collectionId), item);

						return JsonConvert.DeserializeObject<T> (result.Resource.ToString ());

					default: throw;
				}
			}
			catch (Exception ex)
			{
				Log.Debug (ex.Message);
				throw;
			}
			finally
			{
				updateNetworkActivityIndicator (false);
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
				if (client == null) await refreshResourceToken<T> (false);
				//if (!IsInitialized (collectionId)) await InitializeCollection (collectionId);

				updateNetworkActivityIndicator (true);

				var result = await client.ReplaceDocumentAsync (item.SelfLink, item);

				return JsonConvert.DeserializeObject<T> (result.Resource.ToString ());
			}
			catch (DocumentClientException dex)
			{
				dex.Print ();

				switch (dex.StatusCode)
				{
					case HttpStatusCode.Forbidden:

						await refreshResourceToken<T> ();

						var result = await client.ReplaceDocumentAsync (item.SelfLink, item);

						return JsonConvert.DeserializeObject<T> (result.Resource.ToString ());

					default: throw;
				}
			}
			catch (Exception ex)
			{
				Log.Debug (ex.Message);
				throw;
			}
			finally
			{
				updateNetworkActivityIndicator (false);
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
				if (client == null) await refreshResourceToken<T> (false);
				//if (!IsInitialized (collectionId)) await InitializeCollection (collectionId);

				updateNetworkActivityIndicator (true);

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

				switch (dex.StatusCode)
				{
					case HttpStatusCode.Forbidden:

						await refreshResourceToken<T> ();

						var result = await client.DeleteDocumentAsync (item.SelfLink);

						if (!string.IsNullOrEmpty (result?.Resource?.ToString ()))
						{
							return JsonConvert.DeserializeObject<T> (result.Resource.ToString ());
						}

						return null;

					default: throw;
				}
			}
			catch (Exception ex)
			{
				Log.Debug (ex.Message);
				throw;
			}
			finally
			{
				updateNetworkActivityIndicator (false);
			}
		}

		#endregion


		void updateNetworkActivityIndicator (bool visible)
		{
#if __IOS__
			UIKit.UIApplication.SharedApplication.BeginInvokeOnMainThread (() => UIKit.UIApplication.SharedApplication.NetworkActivityIndicatorVisible = visible);
#endif
		}


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
					updateNetworkActivityIndicator (true);

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
					switch (dex.StatusCode)
					{
						case HttpStatusCode.NotFound:

							_databaseTask = client.CreateDatabaseAsync (new Database { Id = databaseId });

							var database = await _databaseTask;

							if (database?.Resource != null)
							{
								_databaseStatus = ClientStatus.Initialized;

								Log.Debug ($"Created new Database");
							}

							break;

						case HttpStatusCode.Forbidden:

							if (_databaseStatus == ClientStatus.NotInitialized)
							{
								_databaseStatus = ClientStatus.Initializing;

								//await refreshResourceToken<> ();

								_databaseTask = null;

								await createDatabaseIfNotExistsAsync ();
							}
							else
							{
								_databaseStatus = ClientStatus.NotInitialized;
								throw;
							}

							break;

						default:
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
				finally
				{
					updateNetworkActivityIndicator (false);
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
					updateNetworkActivityIndicator (true);

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
					switch (dex.StatusCode)
					{
						case HttpStatusCode.NotFound:

							_collectionCreationTasks [collectionId] = client.CreateDocumentCollectionAsync (UriFactory.CreateDatabaseUri (databaseId), new DocumentCollection { Id = collectionId }, new RequestOptions { OfferThroughput = 1000 });

							var collection = await _collectionCreationTasks [collectionId];

							if (collection?.Resource != null)
							{
								_collectionStatuses [collectionId] = ClientStatus.Initialized;
								Log.Debug ($"Created new Collection: {collectionId}");
							}

							break;
						case HttpStatusCode.Forbidden:

							if (_collectionStatuses.TryGetValue (collectionId, out ClientStatus status) && status == ClientStatus.NotInitialized)
							{
								// set to initializing so we don't recurse more than once
								_collectionStatuses [collectionId] = ClientStatus.Initializing;

								//await refreshResourceToken ();

								_collectionCreationTasks [collectionId] = null;

								await createCollectionIfNotExistsAsync (collectionId);
							}
							else
							{
								_collectionStatuses [collectionId] = ClientStatus.NotInitialized;
								throw;
							}

							break;

						default:
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
				finally
				{
					updateNetworkActivityIndicator (false);
				}
			}
		}

		#endregion
	}
}
