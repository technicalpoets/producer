using System;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.WebJobs.Host;

using Producer.Domain;

namespace Producer.Functions
{
	public static class DocumentUpdateNotifier
	{
		[FunctionName ("UpdateNotifier")]
		public static async Task Run (
			[QueueTrigger ("message-queue-document-update", Connection = "AzureWebJobsStorage")]DocumentUpdatedMessage updateMessage,
			[NotificationHub (ConnectionStringSetting = "AzureNotificationHubConnection", HubName = "producer", Platform = NotificationPlatform.Apns, TagExpression = "")] IAsyncCollector<Notification> notification,
			TraceWriter log)
		{
			log.Info ("new DocumentUpdatedMessage");
			log.Info (Newtonsoft.Json.JsonConvert.SerializeObject (updateMessage));

			try
			{
				if (string.IsNullOrEmpty (updateMessage.CollectionId))
					throw new ArgumentException ("Must have value set for CollectionId", nameof (updateMessage));


				var payload = ApsPayload.Create (updateMessage.Title, updateMessage.Message, updateMessage.CollectionId).Serialize ();


				log.Info ($"Sending Notification payload: {payload}");

				await notification.AddAsync (new AppleNotification (payload));
			}
			catch (Exception ex)
			{
				log.Error (ex.Message);
				throw;
			}
		}
	}
}
