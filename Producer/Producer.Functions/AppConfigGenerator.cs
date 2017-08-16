using System;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace Producer.Functions
{
	public class ProducerConfig
	{
		public string UseLocalFunctions { get; set; }
		public string LocalFunctionsUrl { get; set; }
		public string LocalDocumentDbKey { get; set; }
		public string RemoteFunctionsUrl { get; set; }
		public string UseLocalDocumentDb { get; set; }
		public string LocalDocumentDbUrl { get; set; }
		public string RemoteDocumentDbUrl { get; set; }
		public string RemoteDocumentDbKey { get; set; }
		public string EmbeddedSocialKey { get; set; }
		public string NotificationsName { get; set; }
		public string NotificationsUrl { get; set; }
		public string NotificationsKey { get; set; }
		public string NotificationsConnectionString { get; set; }
		public string MobileCenterKey { get; set; }
	}

	public static class AppConfigGenerator
	{
		[FunctionName ("GetAppConfig")]
		public static string Run (
			[HttpTrigger (AuthorizationLevel.Anonymous, "get", Route = "config")]HttpRequestMessage req,
			//[DocumentDB ("Content", "{collectionId}", Id = "{documentId}")] Content content,
			//string collectionId,
			//string documentId,
			TraceWriter log)
		{

			var config = new ProducerConfig
			{
				UseLocalFunctions = "UseLocalFunctions",
				LocalFunctionsUrl = "LocalFunctionsUrl",
				LocalDocumentDbKey = "LocalDocumentDbKey",
				RemoteFunctionsUrl = "RemoteFunctionsUrl",
				UseLocalDocumentDb = "UseLocalDocumentDb",
				LocalDocumentDbUrl = "LocalDocumentDbUrl",
				RemoteDocumentDbUrl = "RemoteDocumentDbUrl",
				RemoteDocumentDbKey = "RemoteDocumentDbKey",
				EmbeddedSocialKey = "EmbeddedSocialKey",
				NotificationsName = "NotificationsName",
				NotificationsUrl = "NotificationsUrl",
				NotificationsKey = "NotificationsKey",
				NotificationsConnectionString = "NotificationsConnectionString",
				MobileCenterKey = "MobileCenterKey",
			};

			return JsonConvert.SerializeObject (config);
		}
	}
}
