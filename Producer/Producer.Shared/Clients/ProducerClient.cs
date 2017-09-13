using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Producer.Auth;
using Producer.Domain;

namespace Producer.Shared
{
	public class ProducerClient
	{
		static readonly object _object = new object ();

		static ProducerClient _shared;
		public static ProducerClient Shared => _shared ?? (_shared = new ProducerClient ());

		static AuthUserConfig authUser;


		static Keychain _keychain;

		Keychain Keychain
		{
			get
			{
				lock (_object)
				{
					return _keychain ?? (_keychain = new Keychain ());
				}
			}
		}


		static User _user;

		public User User
		{
			get
			{
				lock (_object)
				{
					if (_user == null)
					{
						if (ClientAuthManager.Shared.ClientAuthDetails != null)
						{
							authUser = AuthUserConfig.FromKeychain (Keychain);

							if (authUser != null)
							{
								_user = new User (ClientAuthManager.Shared.ClientAuthDetails, authUser);
							}
						}
					}
					return _user;
				}
			}
		}


		public bool Initialized => _httpClient != null;


		public UserRoles UserRole => User?.UserRole ?? UserRoles.General;

		public event EventHandler<User> CurrentUserChanged;


		HttpClient _httpClient;
		HttpClient HttpClient
		{
			get
			{
				if (_httpClient == null)
				{
					_httpClient = new HttpClient { BaseAddress = Settings.FunctionsUrl };

					var storedKeys = Keychain.GetItemFromKeychain (AzureAppServiceUser.AuthenticationHeader);

					if (!string.IsNullOrEmpty (storedKeys.Account) && !string.IsNullOrEmpty (storedKeys.PrivateKey))
					{
						_httpClient.DefaultRequestHeaders.Add (AzureAppServiceUser.AuthenticationHeader, storedKeys.PrivateKey);
					}
				}
				return _httpClient;
			}
		}


		ProducerClient () { }


		public Task Publish<T> (T content, UserRoles? publishTo)
			where T : Content => Publish (content, null, null, publishTo);


		public async Task Publish<T> (T content, string notificationTitle = null, string notificationMessage = null, UserRoles? publishTo = null)
			where T : Content
		{
			if (content?.HasId ?? false)
			{
				try
				{
					var updateMessage = new DocumentUpdatedMessage (content.Id, typeof (T).Name, publishTo ?? content.PublishedTo)
					{
						Title = notificationTitle,
						Message = notificationMessage
					};

					Log.Debug (updateMessage.NotificationTags);

					UpdateNetworkActivityIndicator (true);

					var response = await HttpClient.PostAsync (Routes.PublishContent, new StringContent (JsonConvert.SerializeObject (updateMessage), Encoding.UTF8, Routes.Json));

					if (!response.IsSuccessStatusCode)
					{
						throw new Exception ($"Error posting document update message: {updateMessage}");
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
		}


		public async Task<StorageToken> GetStorageToken<T> (T content)
			where T : Content
		{
			var collectionId = typeof (T).Name;

			if (content?.HasId ?? false)
			{
				try
				{
					UpdateNetworkActivityIndicator (true);

					var response = await HttpClient.GetAsync (Routes.StorageToken (collectionId, content.Id));

					var stringContent = await response.Content.ReadAsStringAsync ();

					if (!response.IsSuccessStatusCode || string.IsNullOrEmpty (stringContent))
					{
						throw new Exception ($"Error getting new storage token from server for {collectionId} with Id: {content.Id}");
					}

					return JsonConvert.DeserializeObject<StorageToken> (stringContent);
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

			return null;
		}


		public async Task<string> GetContentToken<T> (bool refresh = false)
			where T : Entity
		{
			const string errorToken = "{\"Message\":\"An error has occurred.\"}";

			var collectionId = typeof (T).Name;

			try
			{
				var resourceToken = Settings.GetContentToken<T> ();

				if (refresh || string.IsNullOrEmpty (resourceToken) || resourceToken == errorToken)
				{
					Log.Info ($"Getting new content token from server for {collectionId}");

					UpdateNetworkActivityIndicator (true);

					var response = await HttpClient.GetAsync (Routes.ContentToken (collectionId));

					var stringContent = await response.Content.ReadAsStringAsync ();

					resourceToken = stringContent.Trim ('"');

					if (!response.IsSuccessStatusCode || resourceToken == errorToken)
					{
						throw new Exception ($"Error getting new content token from server for {collectionId}");
					}

					Settings.SetContentToken<T> (resourceToken);
				}
				else
				{
					Log.Info ($"Found existing content token for {collectionId}");
				}

				return resourceToken;
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


		public void ResetUser ()
		{
			_user = null;

			Keychain.RemoveItemFromKeychain (AzureAppServiceUser.AuthenticationHeader);

			AuthUserConfig.RemoveFromKeychain (Keychain);

			Settings.SetContentToken<AvContent> (string.Empty);

			_httpClient = null;

			ContentClient.Shared.ResetClient ();

			CurrentUserChanged?.Invoke (this, User);
		}


		public async Task AuthenticateUser (string providerToken, string providerAuthCode)
		{
			try
			{
				if (string.IsNullOrEmpty (providerToken)) throw new ArgumentNullException (nameof (providerToken));
				if (string.IsNullOrEmpty (providerAuthCode)) throw new ArgumentNullException (nameof (providerAuthCode));

				ResetUser ();

				var auth = JObject.Parse ($"{{'id_token':'{providerToken}','authorization_code':'{providerAuthCode}'}}").ToString ();

				UpdateNetworkActivityIndicator (true);

				var authResponse = await HttpClient.PostAsync (Routes.LoginGoogle, new StringContent (auth, Encoding.UTF8, Routes.Json));

				if (authResponse.IsSuccessStatusCode)
				{
					var azureUser = JsonConvert.DeserializeObject<AzureAppServiceUser> (await authResponse.Content.ReadAsStringAsync ());

					Keychain.SaveItemToKeychain (AzureAppServiceUser.AuthenticationHeader, "azure", azureUser.AuthenticationToken);

					_httpClient = null;

					var userConfigJson = await HttpClient.GetStringAsync (Routes.AuthenticateUser);

					authUser = JsonConvert.DeserializeObject<AuthUserConfig> (userConfigJson);

					authUser.SaveToKeychain (Keychain);

					Log.Debug (authUser.ToString ());

					_user = null;

					CurrentUserChanged?.Invoke (this, User);
				}
				else
				{
					Log.Error (auth);
					Log.Error (authResponse.ToString ());
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


		public void UpdateNetworkActivityIndicator (bool visible)
		{
#if __IOS__
			UIKit.UIApplication.SharedApplication.BeginInvokeOnMainThread (() => UIKit.UIApplication.SharedApplication.NetworkActivityIndicatorVisible = visible);
#endif
		}
	}
}
