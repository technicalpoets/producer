//using System;
//using System.Net.Http;

//using Microsoft.Azure.WebJobs;
//using Microsoft.Azure.WebJobs.Extensions.Http;
//using Microsoft.Azure.WebJobs.Host;

//using Producer.Domain;

//namespace Producer.Functions
//{
//	public static class AppConfigGenerator
//	{
//		static ProducerSettings _producerSettings;

//		static ProducerSettings ProducerSettings
//		{
//			get
//			{
//				if (_producerSettings == null)
//				{
//					_producerSettings = new ProducerSettings
//					{
//						RemoteFunctionsUrl = Environment.GetEnvironmentVariable ("RemoteFunctionsUrl"),
//						RemoteDocumentDbUrl = Environment.GetEnvironmentVariable ("RemoteDocumentDbUrl"),
//						RemoteDocumentDbKey = Environment.GetEnvironmentVariable ("RemoteDocumentDbKey"),
//						EmbeddedSocialKey = Environment.GetEnvironmentVariable ("EmbeddedSocialKey"),
//						NotificationsName = Environment.GetEnvironmentVariable ("NotificationsName"),
//						NotificationsUrl = Environment.GetEnvironmentVariable ("NotificationsUrl"),
//						NotificationsKey = Environment.GetEnvironmentVariable ("NotificationsKey"),
//						NotificationsConnectionString = Environment.GetEnvironmentVariable ("AzureNotificationHubConnection"),
//						MobileCenterKeyAndroid = Environment.GetEnvironmentVariable ("MobileCenterKeyAndroid"),
//						MobileCenterKeyiOS = Environment.GetEnvironmentVariable ("MobileCenterKeyiOS"),
//					};
//				}

//				return _producerSettings;
//			}
//		}


//		[FunctionName ("GetProducerSettings")]
//		public static ProducerSettings Run ([HttpTrigger (AuthorizationLevel.Anonymous, "get", Route = "settings")] HttpRequestMessage req, TraceWriter log)
//		{
//			log.Info (ProducerSettings.ToString ());

//			return ProducerSettings;
//		}
//	}
//}
