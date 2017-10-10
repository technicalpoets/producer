using System;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.WebJobs.Host;

using Producer.Domain;

namespace Producer.Functions
{
	public static class NotifyClients
	{

		[FunctionName (nameof (NotifyClients))]
		public static async Task Run (
			[QueueTrigger (MessageQueues.DocumentUpdate, Connection = EnvironmentVariables.AzureWebJobsStorage)] DocumentUpdatedMessage updateMessage,
			[NotificationHub (ConnectionStringSetting = EnvironmentVariables.AzureWebJobsNotificationHubsConnectionString, /*Platform = NotificationPlatform.Apns,*/ TagExpression = "{NotificationTags}")] IAsyncCollector<Notification> notification,
			TraceWriter log)
		{
			try
			{
				log.Info (updateMessage?.ToString ());

				FunctionExtensions.HasValueOrThrow (updateMessage?.CollectionId, DocumentUpdatedMessage.CollectionIdKey);

				var template = PushTemplate.FromMessage (updateMessage);

				await notification.AddAsync (new TemplateNotification (template.GetProperties ()));
			}
			catch (Exception ex)
			{
				log.Error (ex.Message);
				throw;
			}
		}
	}
}
