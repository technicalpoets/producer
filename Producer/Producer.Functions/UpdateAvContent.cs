using System;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

using Producer.Domain;

namespace Producer.Functions
{
	[StorageAccount (EnvironmentVariables.AzureWebJobsStorage)]
	public static class UpdateAvContent
	{
		[FunctionName (nameof (UpdateAvContent))]
		public static void Run (
			[QueueTrigger (MessageQueues.AvContent)] ContentEncodedMessage contentMessage,
			[DocumentDB (nameof (Content), nameof (AvContent), Id = "{documentId}")] AvContent avContent,
			[Queue (MessageQueues.DocumentUpdate)] out DocumentUpdatedMessage updatedMessage, TraceWriter log)
		{
			try
			{
				log.Info (contentMessage.ToString ());

				FunctionExtensions.HasValueOrThrow (avContent?.Id, "avContent", $"Unable to find record with Id: {contentMessage?.DocumentId}");

				FunctionExtensions.HasValueOrThrow (contentMessage?.RemoteAssetUri, "RemoteAssetUri");

				avContent.RemoteAssetUri = contentMessage.RemoteAssetUri;

				updatedMessage = new DocumentUpdatedMessage (contentMessage.DocumentId, contentMessage.CollectionId, contentMessage.NotificationTags)
				{
					Title = $"New {avContent.ContentType}!",
					Message = avContent.DisplayName
				};
			}
			catch (Exception ex)
			{
				log.Error (ex.Message, ex);
				throw;
			}
		}
	}
}
