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

using Producer.Domain;


namespace Producer.Shared
{
	public class ContentClient
	{

		static ContentClient _shared;
		public static ContentClient Shared => _shared ?? (_shared = new ContentClient (nameof (Content)));


		readonly string databaseId;

		DocumentClient client;

		bool initialDataLoad;

		public bool Initialized => client != null && initialDataLoad;


		public UserRoles UserRole => ProducerClient.Shared.UserRole;

		public Dictionary<UserRoles, List<AvContent>> AvContent = new Dictionary<UserRoles, List<AvContent>> {
			{ UserRoles.General,  new List<AvContent>() },
			{ UserRoles.Insider,  new List<AvContent>() },
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


		async Task RefreshResourceToken<T> (bool forceTokenRefresh = true)
			where T : Entity
		{
			try
			{
				var resourceToken = await ProducerClient.Shared.GetContentToken<T> (forceTokenRefresh);

				ResetClient (resourceToken);
			}
			catch (FormatException)
			{
				var resourceToken = await ProducerClient.Shared.GetContentToken<T> (true);

				ResetClient (resourceToken);
			}
		}


		void ResetClient (string resourceToken)
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
			if (!Initialized && !string.IsNullOrEmpty (Settings.ContentDataCache))
			{
				AvContent = JsonConvert.DeserializeObject<Dictionary<UserRoles, List<AvContent>>> (Settings.ContentDataCache);

				AvContentChanged?.Invoke (this, UserRole);
			}

			await RefreshAvContentAsync ();
		}


		public async Task<AvContent> CreateAvContent (AvContent item)
		{
			var newItem = await Create (item);

			AvContent [newItem.PublishedTo].Insert (0, newItem);

			AvContentChanged?.Invoke (this, newItem.PublishedTo);

			return newItem;
		}


		public async Task UpdateAvContent (AvContent item, UserRoles? oldRole = null, bool publish = true)
		{
			if (oldRole.HasValue && AvContent [oldRole.Value].Remove (item))
			{
				// specify old role if role is what changed
				AvContentChanged?.Invoke (this, oldRole.Value);
			}
			else if (AvContent [item.PublishedTo].Remove (item))
			{
				AvContentChanged?.Invoke (this, item.PublishedTo);
			}

			var newItem = await Replace (item);

			AvContent [newItem.PublishedTo].Add (newItem);

			AvContentChanged?.Invoke (this, newItem.PublishedTo);

			if (publish)
			{
				await PublishUpdate (newItem, oldRole);
			}
		}


		async Task PublishUpdate (AvContent newItem, UserRoles? oldRole = null)
		{
			if (oldRole.HasValue)
			{
				if (oldRole.Value < newItem.PublishedTo)
				{
					// moved to more restricted send silent notificaiton to remove from list
					await ProducerClient.Shared.Publish (newItem, oldRole);
				}
				else if (oldRole.Value > newItem.PublishedTo && newItem.PublishedTo < UserRoles.Producer) // published to more users
				{
					var groupName = newItem.PublishedTo != UserRoles.General ? $" ({newItem.PublishedTo})" : string.Empty;

					await ProducerClient.Shared.Publish (newItem, newItem.DisplayName, $"New {newItem.ContentType}!{groupName}");
				}
			}
			else // not adding to or removing from any group, silently update
			{
				await ProducerClient.Shared.Publish (newItem);
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


		async Task RefreshAvContentAsync ()
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

				var content = await Get (predicate);

				AvContent = content.GroupBy (c => c.PublishedTo).ToDictionary (g => g.Key, g => g.OrderByDescending (i => i.Timestamp).ToList ());

				if (!AvContent.ContainsKey (UserRoles.General)) AvContent [UserRoles.General] = new List<AvContent> ();
				if (!AvContent.ContainsKey (UserRoles.Insider)) AvContent [UserRoles.Insider] = new List<AvContent> ();
				if (!AvContent.ContainsKey (UserRoles.Producer)) AvContent [UserRoles.Producer] = new List<AvContent> ();


				initialDataLoad = true;

				AvContentChanged?.Invoke (this, UserRole);

				Settings.ContentDataCache = JsonConvert.SerializeObject (AvContent);
			}
			catch (Exception ex)
			{
				Log.Debug (ex.Message);
				throw;
			}
		}


		#region CRUD


		public Task<T> Get<T> (string id, string collectionId = null)
			where T : Entity
		{
			return ExecuteWithRetry<T> (() => client.ReadDocumentAsync (UriFactory.CreateDocumentUri (databaseId, collectionId ?? typeof (T).Name, id)));
		}


		public Task<List<T>> Get<T> (Expression<Func<T, bool>> predicate, string collectionId = null)
			where T : Entity
		{
			return ExecuteWithRetry (() => GetList (predicate, collectionId));
		}


		public Task<T> Create<T> (T item, string collectionId = null)
			where T : Entity
		{
			return ExecuteWithRetry<T> (() => client.CreateDocumentAsync (UriFactory.CreateDocumentCollectionUri (databaseId, collectionId ?? typeof (T).Name), item));
		}


		public Task<T> Replace<T> (T item)
			where T : Entity
		{
			return ExecuteWithRetry<T> (() => client.ReplaceDocumentAsync (item.SelfLink, item));
		}


		public Task<T> Delete<T> (T item)
			where T : Entity
		{
			return ExecuteWithRetry<T> (() => client.DeleteDocumentAsync (item.SelfLink));
		}


		async Task<List<T>> GetList<T> (Expression<Func<T, bool>> predicate, string collectionId = null)
			where T : Entity
		{
			var results = new List<T> ();

			var query = client.CreateDocumentQuery<T> (UriFactory.CreateDocumentCollectionUri (databaseId, collectionId ?? typeof (T).Name), new FeedOptions { MaxItemCount = -1 })
				  .Where (predicate)
				  .AsDocumentQuery ();

			while (query.HasMoreResults)
			{
				results.AddRange (await query.ExecuteNextAsync<T> ());
			}

			return results;
		}


		async Task<T> ExecuteWithRetry<T> (Func<Task<ResourceResponse<Document>>> task)
			where T : Entity
		{
			try
			{
				if (client == null) await RefreshResourceToken<T> (false);

				UpdateNetworkActivityIndicator (true);

				return Deserialize<T> (await task ());
			}
			catch (DocumentClientException dex)
			{
				Log.Debug (dex.Print ());

				switch (dex.StatusCode)
				{
					case HttpStatusCode.NotFound: return null;
					case HttpStatusCode.Forbidden:

						await RefreshResourceToken<T> ();

						return Deserialize<T> (await task ());

					default: throw;
				}
			}
			catch (Exception ex)
			{
				Log.Error (ex);
				throw;
			}
			finally
			{
				UpdateNetworkActivityIndicator (false);
			}
		}


		async Task<List<T>> ExecuteWithRetry<T> (Func<Task<List<T>>> task)
			where T : Entity
		{
			try
			{
				if (client == null) await RefreshResourceToken<T> (false);

				UpdateNetworkActivityIndicator (true);

				return await task ();
			}
			catch (DocumentClientException dex)
			{
				Log.Debug (dex.Print ());

				switch (dex.StatusCode)
				{
					case HttpStatusCode.NotFound: return null;
					case HttpStatusCode.Forbidden:

						await RefreshResourceToken<T> ();

						return await task ();

					default: throw;
				}
			}
			catch (Exception ex)
			{
				Log.Error (ex);
				throw;
			}
			finally
			{
				UpdateNetworkActivityIndicator (false);
			}
		}


		T Deserialize<T> (ResourceResponse<Document> result)
			where T : Entity
		{
			var json = result?.Resource?.ToString ();

			return string.IsNullOrEmpty (json) ? null : JsonConvert.DeserializeObject<T> (json);
		}


		#endregion


		void UpdateNetworkActivityIndicator (bool visible)
		{
#if __IOS__
			UIKit.UIApplication.SharedApplication.BeginInvokeOnMainThread (() => UIKit.UIApplication.SharedApplication.NetworkActivityIndicatorVisible = visible);
#endif
		}
	}
}
