using System;

using System.Threading;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

using Microsoft.WindowsAzure.MediaServices.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

using Producer.Domain;

namespace Producer.Functions
{
	[StorageAccount (EnvironmentVariables.AzureWebJobsStorage)]
	public static class EncodeBlob
	{

		const string encoderProcessorName = "Media Encoder Standard";
		const string encoderTaskConfigName = "H264 Multiple Bitrate 16x9 for iOS";

		static CloudMediaContext Context;
		static CloudStorageAccount StorageAccount;
		static MediaServicesCredentials CachedCredentials;


		[FunctionName (nameof (EncodeBlob))]
		public static void Run (
			[BlobTrigger (Routes.EncodeBlob)] CloudBlockBlob inputBlob, string fileName, string fileExtension,
			[Queue (MessageQueues.AvContent)] out ContentEncodedMessage contentMessage, TraceWriter log)
		{
			try
			{
				contentMessage = FunctionExtensions.GetMessageOrThrow (inputBlob);

				CachedCredentials = new MediaServicesCredentials (EnvironmentVariables.MediaServicesAccountName, EnvironmentVariables.MediaServicesAccountKey);

				Context = new CloudMediaContext (CachedCredentials);


				var newAsset = CreateAssetFromBlob (inputBlob, log).GetAwaiter ().GetResult ();

				var newAssetName = GetEncodedAssetName (fileName, fileExtension, encoderProcessorName);


				var job = Context.Jobs.CreateWithSingleTask (encoderProcessorName, encoderTaskConfigName, newAsset, newAssetName, AssetCreationOptions.None);

				job.Submit ();


				job = job.StartExecutionProgressTask (j => log.Info ($"Encoding Job Id: {job.Id}  State: {job.State}  Progress: {j.GetOverallProgress ().ToString ("P")}"), CancellationToken.None).Result;


				switch (job.State)
				{
					case JobState.Finished:
						log.Info ($"Encoding Job Id: {job.Id} is complete.");
						break;
					case JobState.Error: throw new Exception ($"Encoding Job Id: {job.Id} failed.");
				}


				var outputAsset = job.OutputMediaAssets [0];

				Context.Locators.Create (LocatorType.OnDemandOrigin, outputAsset, AccessPermissions.Read, TimeSpan.FromDays (365 * 10), DateTime.UtcNow);


				contentMessage.SetRemoteAssetUri (outputAsset.GetHlsUri ());


				log.Info ($"Output Asset - {contentMessage}");
			}
			catch (Exception ex)
			{
				log.Error ($"ERROR: failed with exception {ex.Message}.\n {ex.StackTrace}");
				throw;
			}
		}


		static string GetEncodedAssetName (string fileName, string fileExtension, string processorName) => $"{fileName}.{fileExtension} - {processorName} encoded";


		static async Task<IAsset> CreateAssetFromBlob (CloudBlockBlob blob, TraceWriter log)
		{
			IAsset newAsset = null;

			try
			{
				Task<IAsset> copyAssetTask = CreateAssetFromBlobAsync (blob, log);
				newAsset = await copyAssetTask;
			}
			catch (Exception ex)
			{
				log.Error ($"ERROR: Copy failed with exception {ex.Message}\n{ex.StackTrace}");
				throw;
			}

			return newAsset;
		}


		static async Task<IAsset> CreateAssetFromBlobAsync (CloudBlockBlob blob, TraceWriter log)
		{
			StorageAccount = CloudStorageAccount.Parse (EnvironmentVariables.StorageAccountConnection);

			var blobClient = StorageAccount.CreateCloudBlobClient ();

			var writePolicy = Context.AccessPolicies.Create ("writePolicy", TimeSpan.FromHours (24), AccessPermissions.Write);


			var asset = Context.Assets.Create (blob.Name, AssetCreationOptions.None);


			var assetLocator = Context.Locators.CreateLocator (LocatorType.Sas, asset, writePolicy);

			var assetContainer = blobClient.GetContainerReference ((new Uri (assetLocator.Path)).Segments [1]);

			try
			{
				assetContainer.CreateIfNotExists ();
			}
			catch (Exception ex)
			{
				log.Error ($"ERROR: Blob container creation failed with exception {ex.Message}\n{ex.StackTrace}");
				throw;
			}

			var assetBlob = assetContainer.GetBlockBlobReference (blob.Name);

			var sasBlobToken = blob.GetSharedAccessSignature (AdHocSasPolicy);

			try
			{
				log.Info ("Starting blob copy.");

				await assetBlob.StartCopyAsync (new Uri (blob.Uri + sasBlobToken));

				log.Info ("Blob copy complete.");

				var assetFile = asset.AssetFiles.Create (blob.Name);

				assetFile.ContentFileSize = blob.Properties.Length;

				assetFile.Update ();

				asset.Update ();
			}
			catch (Exception ex)
			{
				log.Error ($"ERROR: Copy failed with exception {ex.Message}\n{ex.StackTrace}");
				throw;
			}

			assetLocator.Delete ();
			writePolicy.Delete ();

			return asset;
		}


		static SharedAccessBlobPolicy AdHocSasPolicy => new SharedAccessBlobPolicy
		{
			SharedAccessExpiryTime = DateTime.UtcNow.AddHours (24),
			Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.Create
		};
	}
}