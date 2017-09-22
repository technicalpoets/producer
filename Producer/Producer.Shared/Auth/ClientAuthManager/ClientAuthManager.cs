//#define NC_AUTH_GOOGLE
//#define NC_AUTH_FACEBOOK
//#define NC_AUTH_MICROSOFT
//#define NC_AUTH_TWITTER

using System;

#if __IOS__
using UIKit;
using Foundation;
#elif __ANDROID__
using Android.Support.V4.App;
#endif

namespace Producer.Auth
{
	public partial class ClientAuthManager
	{
		static readonly object _object = new object ();

		Keychain _keychain;
		Keychain Keychain => _keychain ?? (_keychain = new Keychain ());

		static string KeychainServiceName (ClientAuthProviders provider, ClientAuthDetailTypes type)
		{
			return $"{provider.ToString ().ToLower ()}.{type.ToString ().ToLower ()}";
		}

		static ClientAuthManager _shared;
		public static ClientAuthManager Shared => _shared ?? (_shared = new ClientAuthManager ());


		ClientAuthManager () { }


		public event EventHandler<ClientAuthDetails> AuthorizationChanged;


		ClientAuthDetails _clientAuthDetails;
		public ClientAuthDetails ClientAuthDetails
		{
			get
			{
				lock (_object)
				{
					if (_clientAuthDetails == null)
					{
						_clientAuthDetails = initClientAuthDetails () ?? null;
					}

					return _clientAuthDetails;
				}
			}
			private set
			{
				ClientAuthProviders? provider = _clientAuthDetails?.ClientAuthProvider;

				_clientAuthDetails = value;

				if (_clientAuthDetails == null)
				{
					if (provider.HasValue)
					{
						removeProviderKeychainData (provider.Value);
					}
				}
				else
				{
					saveClientAuthDetailsToKeychain (_clientAuthDetails);
				}
			}
		}


		public void SetClientAuthDetails (ClientAuthDetails details)
		{
			Log.Debug (details?.ToString ());

			ClientAuthDetails = details;

			AuthorizationChanged?.Invoke (this, ClientAuthDetails);
		}


		public void LogoutAuthProviders ()
		{
			logoutAuthProviderGoogle ();
			logoutAuthProviderFacebook ();
			logoutAuthProviderMicrosoft ();
			logoutAuthProviderTwitter ();

			var providers = new [] { ClientAuthProviders.Google, ClientAuthProviders.Facebook, ClientAuthProviders.Microsoft, ClientAuthProviders.Twitter };

			foreach (var provider in providers)
			{
				removeProviderKeychainData (provider);
			}

			SetClientAuthDetails (null);
		}


		void removeProviderKeychainData (ClientAuthProviders provider)
		{
			removeItemFromKeychain (KeychainServiceName (provider, ClientAuthDetailTypes.Token));
			removeItemFromKeychain (KeychainServiceName (provider, ClientAuthDetailTypes.Name));
			removeItemFromKeychain (KeychainServiceName (provider, ClientAuthDetailTypes.GivenName));
			removeItemFromKeychain (KeychainServiceName (provider, ClientAuthDetailTypes.FamilyName));
			removeItemFromKeychain (KeychainServiceName (provider, ClientAuthDetailTypes.Username));
			removeItemFromKeychain (KeychainServiceName (provider, ClientAuthDetailTypes.Email));
			removeItemFromKeychain (KeychainServiceName (provider, ClientAuthDetailTypes.AuthCode));
			removeItemFromKeychain (KeychainServiceName (provider, ClientAuthDetailTypes.AvatarUrl));
		}


		void saveClientAuthDetailsToKeychain (ClientAuthDetails details)
		{
			if (!string.IsNullOrEmpty (details?.Token)) saveItemToKeychain (KeychainServiceName (details.ClientAuthProvider, ClientAuthDetailTypes.Token), ClientAuthDetailTypes.Token.ToString (), details.Token);
			if (!string.IsNullOrEmpty (details?.Name)) saveItemToKeychain (KeychainServiceName (details.ClientAuthProvider, ClientAuthDetailTypes.Name), ClientAuthDetailTypes.Name.ToString (), details.Name);
			if (!string.IsNullOrEmpty (details?.GivenName)) saveItemToKeychain (KeychainServiceName (details.ClientAuthProvider, ClientAuthDetailTypes.GivenName), ClientAuthDetailTypes.GivenName.ToString (), details.GivenName);
			if (!string.IsNullOrEmpty (details?.FamilyName)) saveItemToKeychain (KeychainServiceName (details.ClientAuthProvider, ClientAuthDetailTypes.FamilyName), ClientAuthDetailTypes.FamilyName.ToString (), details.FamilyName);
			if (!string.IsNullOrEmpty (details?.Username)) saveItemToKeychain (KeychainServiceName (details.ClientAuthProvider, ClientAuthDetailTypes.Username), ClientAuthDetailTypes.Username.ToString (), details.Username);
			if (!string.IsNullOrEmpty (details?.Email)) saveItemToKeychain (KeychainServiceName (details.ClientAuthProvider, ClientAuthDetailTypes.Email), ClientAuthDetailTypes.Email.ToString (), details.Email);
			if (!string.IsNullOrEmpty (details?.AuthCode)) saveItemToKeychain (KeychainServiceName (details.ClientAuthProvider, ClientAuthDetailTypes.AuthCode), ClientAuthDetailTypes.AuthCode.ToString (), details.AuthCode);
			if (!string.IsNullOrEmpty (details?.AvatarUrl)) saveItemToKeychain (KeychainServiceName (details.ClientAuthProvider, ClientAuthDetailTypes.AvatarUrl), ClientAuthDetailTypes.AvatarUrl.ToString (), details.AvatarUrl);
		}

#if __IOS__

		public void InitializeAuthProviders (UIApplication application, NSDictionary launchOptions)
		{
			initializeAuthProviderGoogle ();
			initializeAuthProviderFacebook (application, launchOptions);
		}


		public bool OpenUrl (UIApplication app, NSUrl url, NSDictionary options)
		{
			var openUrlOptions = new UIApplicationOpenUrlOptions (options);

			return openUrlGoogle (app, url, openUrlOptions) || openUrlFacebook (app, url, openUrlOptions);
		}

#elif __ANDROID__

		public int AuthActivityLayoutResId { get; set; }


		public void InitializeAuthProviders<T> (T context)
			where T : FragmentActivity
#if NC_AUTH_GOOGLE
				, Android.Gms.Common.Apis.GoogleApiClient.IOnConnectionFailedListener
#endif
		{
			initializeAuthProviderGoogle (context);
		}

#endif


		ClientAuthDetails initClientAuthDetails ()
		{
			var providers = new [] { ClientAuthProviders.Google, ClientAuthProviders.Facebook, ClientAuthProviders.Microsoft, ClientAuthProviders.Twitter };

			var clientAuthDetails = new ClientAuthDetails ();

			ClientAuthProviders? provider = null;

			string token = null;// = getItemFromKeychain (KeychainServiceName (ClientAuthProviders.Google, ClientAuthDetailTypes.Token));

			foreach (var item in providers)
			{
				token = getItemFromKeychain (KeychainServiceName (item, ClientAuthDetailTypes.Token)).PrivateKey;

				if (!string.IsNullOrEmpty (token))
				{
					provider = item;

					break;
				}
			}

			if (provider.HasValue)
			{
				Log.Debug ($"Existing ClientAuthProvider found: {provider.Value}");

				clientAuthDetails.ClientAuthProvider = provider.Value;
				clientAuthDetails.Token = token;
				clientAuthDetails.Name = getItemFromKeychain (KeychainServiceName (provider.Value, ClientAuthDetailTypes.Name)).PrivateKey;
				clientAuthDetails.GivenName = getItemFromKeychain (KeychainServiceName (provider.Value, ClientAuthDetailTypes.GivenName)).PrivateKey;
				clientAuthDetails.FamilyName = getItemFromKeychain (KeychainServiceName (provider.Value, ClientAuthDetailTypes.FamilyName)).PrivateKey;
				clientAuthDetails.Username = getItemFromKeychain (KeychainServiceName (provider.Value, ClientAuthDetailTypes.Username)).PrivateKey;
				clientAuthDetails.Email = getItemFromKeychain (KeychainServiceName (provider.Value, ClientAuthDetailTypes.Email)).PrivateKey;
				clientAuthDetails.AuthCode = getItemFromKeychain (KeychainServiceName (provider.Value, ClientAuthDetailTypes.AuthCode)).PrivateKey;
				clientAuthDetails.AvatarUrl = getItemFromKeychain (KeychainServiceName (provider.Value, ClientAuthDetailTypes.AvatarUrl)).PrivateKey;

				return clientAuthDetails;
			}

			return null;
		}

		(string Account, string PrivateKey) getItemFromKeychain (string service) => Keychain.GetItemFromKeychain (service);

		bool saveItemToKeychain (string service, string account, string privateKey) => Keychain.SaveItemToKeychain (service, account, privateKey);

		bool removeItemFromKeychain (string service) => Keychain.RemoveItemFromKeychain (service);
	}
}
