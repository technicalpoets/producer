//using System;
//using System.Collections.Generic;
//using System.Net.Http;
//using System.Threading.Tasks;
//using System.Linq;

////using NomadCode.Azure;

//using Producer.Domain;

//namespace Producer.Shared
//{
//	public class ProducerClient
//	{
//		static ProducerClient _shared;
//		public static ProducerClient Shared => _shared ?? (_shared = new ProducerClient ());


//		UserRoles UserRole;


//		//AzureClient azureClient => AzureClient.Shared;


//		public Dictionary<UserRoles, List<AvContent>> AvContent = new Dictionary<UserRoles, List<AvContent>> {
//			{ UserRoles.General, new List<AvContent>() },
//			{ UserRoles.Insider, new List<AvContent>() },
//			{ UserRoles.Producer, new List<AvContent>() }
//		};


//		public event EventHandler<UserRoles> AvContentChanged;


//		public async Task GetAllAvContentAsync (bool forceSync = false, UserRoles userRole = UserRoles.General)
//		{
//			UserRole = userRole;

//			if (forceSync || AvContent [UserRole]?.Count < 1)
//			{
//				//await azureClient.SyncAsync<AvContent> ();
//			}

//			await refreshAvContentAsync ();

//			//#if DEBUG
//			//			if (AvContent [userRole]?.Count < 1)
//			//			{
//			//				await SeedDatabase (userRole);
//			//			}
//			//#endif
//		}


//		public async Task UpdateItemAsync (AvContent avContent, bool waitOnSync = false)
//		{
//			if (waitOnSync)
//			{
//				//await azureClient.SaveRemoteAsync (avContent);
//			}
//			else
//			{
//				//await azureClient.SaveAsync (avContent);
//			}

//			await refreshAvContentAsync ();
//		}


//		public async Task<StorageToken> SaveNewItemAsync (AvContent avContent)
//		{
//			//await AzureClient.Shared.SaveRemoteAsync (avContent);

//			if (avContent?.HasId ?? false)
//			{
//				//AvContent [UserRoles.Producer].Add (avContent);

//				await refreshAvContentAsync ();

//				var paramDictionary = new Dictionary<string, string> { { StorageToken.ContentIdParam, avContent.Id } };

//				//var storageToken = await azureClient.MobileServiceClient.InvokeApiAsync<StorageToken> (StorageToken.RequestApiName, HttpMethod.Get, paramDictionary);

//				//return storageToken;
//			}

//			return null;
//		}


//		async Task refreshAvContentAsync ()
//		{
//			var content = new List<AvContent> ();

//			switch (UserRole)
//			{
//				case UserRoles.General:
//					//content = await azureClient.GetAsync<AvContent> (c => c.PublishedTo == UserRoles.General);
//					break;
//				case UserRoles.Insider:
//					//content = await azureClient.GetAsync<AvContent> (c => c.PublishedTo == UserRoles.General
//					//|| c.PublishedTo == UserRoles.Insider);
//					break;
//				case UserRoles.Producer:
//				case UserRoles.Admin:
//					//content = await azureClient.GetAsync<AvContent> (c => c.PublishedTo == UserRoles.General
//					//|| c.PublishedTo == UserRoles.Insider
//					//|| c.PublishedTo == UserRoles.Producer);
//					break;
//			}

//			AvContent [UserRoles.General] = content.Where (c => c.PublishedTo == UserRoles.General).OrderByDescending (c => c.UpdatedAt).ToList ();
//			AvContent [UserRoles.Insider] = content.Where (c => c.PublishedTo == UserRoles.Insider).OrderByDescending (c => c.UpdatedAt).ToList ();
//			AvContent [UserRoles.Producer] = content.Where (c => c.PublishedTo == UserRoles.Producer).OrderByDescending (c => c.UpdatedAt).ToList ();

//			AvContentChanged?.Invoke (this, UserRole);
//		}

//		//#if DEBUG
//		//		public async Task SeedDatabase (UserRoles userRole = UserRoles.General)
//		//		{
//		//			System.Diagnostics.Debug.WriteLine ($"Seeding Database");

//		//			await azureClient.SyncAsync<AvContent> ();

//		//			// sorting by updated time, so save them in reverse
//		//			var list = TempData.PublicAvContent.AsEnumerable ().Reverse ().ToList ();

//		//			await azureClient.SaveAsync (list);

//		//			await GetAllAvContentAsync (true, userRole);
//		//		}
//		//#endif
//	}
//}
