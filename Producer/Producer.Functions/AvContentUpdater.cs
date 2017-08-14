using System;
using System.Threading.Tasks;

using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;


using Producer.Domain;

namespace Producer.Functions
{
	public static class AvContentUpdater
	{
		[FunctionName ("UpdateItem")]
		public static async Task Run (
			[QueueTrigger ("message-queue-avcontent", Connection = "AzureWebJobsStorage")]ContentEncodedMessage contentMessage,
			[DocumentDB ("Content", "AvContent", Id = "contentId")] AvContent record,
			[NotificationHub (ConnectionStringSetting = "AzureNotificationHubConnection", HubName = "producer", Platform = NotificationPlatform.Apns, TagExpression = "")] IAsyncCollector<Notification> notification,
			TraceWriter log)
		{
			if (record != null)
			{
				try
				{
					if (string.IsNullOrEmpty (contentMessage.ContentId))
						throw new ArgumentException ("Must have value set for ContentId", nameof (contentMessage));


					if (string.IsNullOrEmpty (contentMessage.RemoteAssetUri))
						throw new ArgumentException ("Must have value set for RemoteAssetUri", nameof (contentMessage));


					record.RemoteAssetUri = contentMessage.RemoteAssetUri;

					//record["RemoteAssetUri"] = contentMessage.RemoteAssetUri;

					// record ["publishedTo"] = "0";

					//var contentName = (string)record["displayName"];
					//var payload = ApsPayload.Create ("New Music!", contentName, true).Serialize ();


					var payload = ApsPayload.Create ("New Music!", record.DisplayName, true).Serialize ();


					log.Info ($"Sending Notification payload: {payload}");


					await notification.AddAsync (new AppleNotification (payload));
				}
				catch (Exception ex)
				{
					log.Error (ex.Message);
					throw;
				}
			}
			else
			{
				var ex = new Exception ($"Unable to find record with Id {contentMessage.ContentId}");
				log.Error (ex.Message);
				throw ex;
			}
		}
	}
}
