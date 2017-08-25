using System;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

using Producer.Domain;

namespace Producer.Functions
{
	[StorageAccount ("AzureWebJobsStorage")]
	public static class AvContentUpdater
	{

		[FunctionName ("UpdateAvContent")]
		public static void Run (
			[QueueTrigger ("message-queue-avcontent")] ContentEncodedMessage contentMessage,
			[DocumentDB ("Content", "AvContent", Id = "{documentId}")] AvContent avContent,
			[Queue ("message-queue-document-update")] out DocumentUpdatedMessage updatedMessage, TraceWriter log)
		{
			log.Info ("new ContentEncodedMessage");
			log.Info (Newtonsoft.Json.JsonConvert.SerializeObject (contentMessage));

			if (avContent != null)
			{
				try
				{
					if (string.IsNullOrEmpty (contentMessage.RemoteAssetUri))
					{
						throw new ArgumentException ("Must have value set for RemoteAssetUri", nameof (contentMessage));
					}


					avContent.RemoteAssetUri = contentMessage.RemoteAssetUri;

					updatedMessage = new DocumentUpdatedMessage (contentMessage.DocumentId, contentMessage.CollectionId)
					{
						Title = $"New {avContent.ContentType}!",
						Message = avContent.DisplayName,
						PublishedTo = (int) avContent.PublishedTo
					};
				}
				catch (Exception ex)
				{
					log.Error (ex.Message);
					throw;
				}
			}
			else
			{
				var ex = new Exception ($"Unable to find record with Id {contentMessage.DocumentId}");
				log.Error (ex.Message);
				throw ex;
			}
		}
	}
}
