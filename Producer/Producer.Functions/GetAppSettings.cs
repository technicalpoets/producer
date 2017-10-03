using System;
using System.Net.Http;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

using HttpStatusCode = System.Net.HttpStatusCode;

namespace Producer.Functions
{
	public static class GetAppSettings
	{
		static Uri DocumentDbUri = EnvironmentVariables.DocumentDbUri;
		static string MobileCenterKey = "";// EnvironmentVariables.MobileCenterKey;
		static string NotificationsName = EnvironmentVariables.NotificationHubName;
		static string NotificationsConnectionString = EnvironmentVariables.AzureWebJobsNotificationHubsConnectionString;


		[FunctionName (nameof (GetAppSettings))]
		public static HttpResponseMessage Run (
			[HttpTrigger (AuthorizationLevel.Anonymous, Routes.Get, Route = Routes.GetAppSettings)] HttpRequestMessage req, TraceWriter log)
		{
			try
			{
				return req.CreateResponse (HttpStatusCode.OK, new AppSettings
				{
					DocumentDbUrl = DocumentDbUri,
					MobileCenterKey = MobileCenterKey,
					NotificationsName = NotificationsName,
					NotificationsConnectionString = NotificationsConnectionString
				});
			}
			catch (Exception ex)
			{
				log.Error (ex.Message, ex);
				return req.CreateErrorResponse (HttpStatusCode.InternalServerError, ex);
			}
		}
	}
}
