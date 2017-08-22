using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using SettingsStudio;

using Producer.Auth;
using Producer.Domain;
using Newtonsoft.Json.Linq;

namespace Producer.Shared
{
	public class ProducerClient
	{
		static ProducerClient _shared;
		public static ProducerClient Shared => _shared ?? (_shared = new ProducerClient ());

		public AuthUserConfig AuthUser { get; set; }

		HttpClient _httpClient;
		HttpClient httpClient => _httpClient ?? (_httpClient = new HttpClient { BaseAddress = Settings.FunctionsUrl });


		ProducerClient () { }


		public async Task Publish<T> (T content, string notificationTitle = null, string notificationMessage = null)
			where T : Content
		{
			if (content?.HasId ?? false)
			{
				var url = $"api/publish";

				try
				{
					var updateMessage = new DocumentUpdatedMessage (content.Id, typeof (T).Name)
					{
						Title = notificationTitle,
						Message = notificationMessage
					};

					var response = await httpClient.PostAsync (url, new StringContent (JsonConvert.SerializeObject (updateMessage), Encoding.UTF8, "application/json"));

					Log.Debug (response.ToString ());

					//var stringContent = await response.Content.ReadAsStringAsync ();

					//return JsonConvert.DeserializeObject<StorageToken> (stringContent);
				}
				catch (Exception ex)
				{
					Log.Debug (ex.Message);
					throw;
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
					//TODO: cache token

					var response = await httpClient.GetAsync (url);

					var stringContent = await response.Content.ReadAsStringAsync ();

					return JsonConvert.DeserializeObject<StorageToken> (stringContent);
				}
				catch (Exception ex)
				{
					Log.Debug (ex.Message);
					throw;
				}
			}

			return null;
		}


		public async Task<string> GetContentToken<T> ()
			where T : Content
		{
			var url = $"api/tokens/read/{typeof (T).Name}";

			try
			{
				var response = await httpClient.GetAsync (url);

				var stringContent = await response.Content.ReadAsStringAsync ();

				return stringContent;
			}
			catch (Exception ex)
			{
				Log.Debug (ex.Message);
				throw;
			}
		}


		public async Task<AuthUserConfig> GetAuthUserConfig ()
		{
			try
			{
#if DEBUG
				if (Settings.UseLocalFunctions)
				{
					httpClient.BaseAddress = new Uri (Settings.RemoteFunctionsUrl);
				}
#endif
				var keychain = new Keychain ();

				var storedKeys = keychain.GetItemFromKeychain (AzureAppServiceUser.AuthenticationHeader);

				if (!string.IsNullOrEmpty (storedKeys.Account) && !string.IsNullOrEmpty (storedKeys.PrivateKey))
				{
					httpClient.DefaultRequestHeaders.Remove (AzureAppServiceUser.AuthenticationHeader);

					httpClient.DefaultRequestHeaders.Add (AzureAppServiceUser.AuthenticationHeader, storedKeys.PrivateKey);

					var userConfigJson = await httpClient.GetStringAsync ("api/user/config");

					//Log.Debug ($"userConfigJson {userConfigJson}");

					AuthUser = JsonConvert.DeserializeObject<AuthUserConfig> (userConfigJson);

					//Log.Debug (AuthUser.ToString ());

					return AuthUser;
				}

				return null;
			}
			catch (HttpRequestException reEx)
			{
				if (reEx.Message.Contains ("401"))
				{
					new Keychain ().RemoveItemFromKeychain (AzureAppServiceUser.AuthenticationHeader);

					return null;
				}

				Log.Error (reEx.Message);
				throw;
			}
			catch (Exception ex)
			{
				Log.Error (ex.Message);
				throw;
			}
#if DEBUG
			finally
			{
				httpClient.BaseAddress = Settings.FunctionsUrl;
			}
#endif
		}


		public async Task<AuthUserConfig> GetAuthUserConfig (string providerToken, string providerAuthCode)
		{
			try
			{
#if DEBUG
				if (Settings.UseLocalFunctions)
				{
					httpClient.BaseAddress = new Uri (Settings.RemoteFunctionsUrl);
				}
#endif

				if (!string.IsNullOrEmpty (providerToken) && !string.IsNullOrEmpty (providerAuthCode))
				{
					var auth = JObject.Parse ($"{{'id_token':'{providerToken}','authorization_code':'{providerAuthCode}'}}").ToString ();

					var authResponse = await httpClient.PostAsync (".auth/login/google", new StringContent (auth, Encoding.UTF8, "application/json"));

					if (authResponse.IsSuccessStatusCode)
					{
						var azureUserJson = await authResponse.Content.ReadAsStringAsync ();

						Log.Debug ($"azureUserJson: {azureUserJson}");

						var azureUser = JsonConvert.DeserializeObject<AzureAppServiceUser> (azureUserJson);


						Log.Debug ($"azureUser.AuthenticationToken {azureUser.AuthenticationToken}");

						httpClient.DefaultRequestHeaders.Remove (AzureAppServiceUser.AuthenticationHeader);

						httpClient.DefaultRequestHeaders.Add (AzureAppServiceUser.AuthenticationHeader, azureUser.AuthenticationToken);

						var keychain = new Keychain ();

						keychain.SaveItemToKeychain (AzureAppServiceUser.AuthenticationHeader, "azure", azureUser.AuthenticationToken);

						var userConfigJson = await httpClient.GetStringAsync ("api/user/config");

						//Log.Debug ($"userConfigJson {userConfigJson}");

						AuthUser = JsonConvert.DeserializeObject<AuthUserConfig> (userConfigJson);

						//Log.Debug (AuthUser.ToString ());

						return AuthUser;
					}
					else
					{
						Log.Error (auth);
						Log.Error (authResponse.ToString ());
					}
				}

				return null;
			}
			catch (Exception ex)
			{
				Log.Error (ex.Message);
				throw;
			}
#if DEBUG
			finally
			{
				httpClient.BaseAddress = Settings.FunctionsUrl;
			}
#endif
		}
	}
}
