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

		static User _user;
		public User User
		{
			get
			{
				lock (_object)
				{
					if (_user == null)
					{
						var clientAuth = ClientAuthManager.Shared.ClientAuthDetails;

						if (clientAuth != null)
						{
							authUser = AuthUserConfig.FromKeychain ();

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


		public UserRoles UserRole => User?.UserRole ?? UserRoles.General;

		public event EventHandler<User> CurrentUserChanged;


		HttpClient _httpClient;
		HttpClient httpClient
		{
			get
			{
				if (_httpClient == null)
				{
					_httpClient = new HttpClient { BaseAddress = Settings.FunctionsUrl };

					var storedKeys = new Keychain ().GetItemFromKeychain (AzureAppServiceUser.AuthenticationHeader);

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
				var url = $"api/publish";

				try
				{
					var updateMessage = new DocumentUpdatedMessage (content.Id, typeof (T).Name, publishTo.HasValue ? publishTo.Value : content.PublishedTo)
					{
						Title = notificationTitle,
						Message = notificationMessage
					};

					Log.Debug (updateMessage.NotificationTags);

					updateNetworkActivityIndicator (true);

					var response = await httpClient.PostAsync (url, new StringContent (JsonConvert.SerializeObject (updateMessage), Encoding.UTF8, "application/json"));

					Log.Debug (response.ToString ());
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
		}


		public async Task<StorageToken> GetStorageToken<T> (T content)
			where T : Content
		{
			if (content?.HasId ?? false)
			{
				var url = $"api/tokens/storage/{typeof (T).Name}/{content.Id}";

				try
				{
					updateNetworkActivityIndicator (true);

					var response = await httpClient.GetAsync (url);

					var stringContent = await response.Content.ReadAsStringAsync ();

					return JsonConvert.DeserializeObject<StorageToken> (stringContent);
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

			return null;
		}


		public async Task<string> GetContentToken<T> (bool refresh = false)
			where T : Entity
		{
			var url = $"api/tokens/content/{typeof (T).Name}";

			try
			{
				var resourceToken = Settings.GetContentToken<T> ();

				if (refresh || string.IsNullOrEmpty (resourceToken))
				{
					Log.Info ($"Getting new content token from server for {typeof (T).Name}");

					updateNetworkActivityIndicator (true);

					var response = await httpClient.GetAsync (url);

					var stringContent = await response.Content.ReadAsStringAsync ();

					resourceToken = stringContent.Trim ('"');

					Settings.SetContentToken<T> (resourceToken);
				}
				else
				{
					Log.Info ($"Found existing content token {typeof (T).Name}");
				}

				return resourceToken;
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


		public void ResetUser ()
		{
			_user = null;

			new Keychain ().RemoveItemFromKeychain (AzureAppServiceUser.AuthenticationHeader);

			AuthUserConfig.RemoveFromKeychain ();

			Settings.SetContentToken<AvContent> (string.Empty);

			_httpClient = null;

			ContentClient.Shared.ResetClient ();

			CurrentUserChanged?.Invoke (this, null);
		}


		public async Task AuthenticateUser (string providerToken, string providerAuthCode)
		{
			try
			{
				if (string.IsNullOrEmpty (providerToken)) throw new ArgumentNullException (nameof (providerToken));
				if (string.IsNullOrEmpty (providerAuthCode)) throw new ArgumentNullException (nameof (providerAuthCode));

				ResetUser ();

				var auth = JObject.Parse ($"{{'id_token':'{providerToken}','authorization_code':'{providerAuthCode}'}}").ToString ();

				updateNetworkActivityIndicator (true);

				var authResponse = await httpClient.PostAsync (".auth/login/google?access_type=offline", new StringContent (auth, Encoding.UTF8, "application/json"));

				if (authResponse.IsSuccessStatusCode)
				{
					var azureUserJson = await authResponse.Content.ReadAsStringAsync ();

					Log.Debug ($"azureUserJson: {azureUserJson}");

					var azureUser = JsonConvert.DeserializeObject<AzureAppServiceUser> (azureUserJson);

					new Keychain ().SaveItemToKeychain (AzureAppServiceUser.AuthenticationHeader, "azure", azureUser.AuthenticationToken);

					_httpClient = null;

					var userConfigJson = await httpClient.GetStringAsync ("api/user/config");

					authUser = JsonConvert.DeserializeObject<AuthUserConfig> (userConfigJson);

					authUser.SaveToKeychain ();

					Log.Debug (authUser.ToString ());

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
				Log.Error (ex.Message);
				throw;
			}
			finally
			{
				updateNetworkActivityIndicator (false);
			}
		}


		public void updateNetworkActivityIndicator (bool visible)
		{
#if __IOS__
			UIKit.UIApplication.SharedApplication.BeginInvokeOnMainThread (() => UIKit.UIApplication.SharedApplication.NetworkActivityIndicatorVisible = visible);
#endif
		}
	}
}
