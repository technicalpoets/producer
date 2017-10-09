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
		static readonly Uri DocumentDbUrl = EnvironmentVariables.DocumentDbUri;
		static readonly string NotificationsName = EnvironmentVariables.NotificationHubName;
		static readonly string NotificationsConnectionString = EnvironmentVariables.NotificationHubConnectionString;


		[FunctionName (nameof (GetAppSettings))]
		public static HttpResponseMessage Run (
			[HttpTrigger (AuthorizationLevel.Anonymous, Routes.Get, Route = Routes.GetAppSettings)] HttpRequestMessage req, TraceWriter log)
		{
			try
			{
				return req.CreateResponse (HttpStatusCode.OK, new AppSettings
				{
					DocumentDbUrl = DocumentDbUrl,
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
