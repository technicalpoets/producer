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
	[StorageAccount ("AzureWebJobsStorage")]
	public static class MediaBlobEncoder
	{

		const string encoderProcessorName = "Media Encoder Standard";
		const string encoderTaskConfigName = "H264 Multiple Bitrate 16x9 for iOS";


		static readonly string _storageAccountConnection = Environment.GetEnvironmentVariable ("AzureWebJobsStorage");
		static readonly string _mediaServicesAccountKey = Environment.GetEnvironmentVariable ("AzureMediaServicesKey");
		static readonly string _mediaServicesAccountName = Environment.GetEnvironmentVariable ("AzureMediaServicesAccount");


		static CloudMediaContext _context;
		static CloudStorageAccount _storageAccount;
		static MediaServicesCredentials _cachedCredentials;


		[FunctionName ("EncodeAvContent")]
		public static void Run (
			[BlobTrigger ("uploads-avcontent/{fileName}.{fileExtension}")] CloudBlockBlob inputBlob, string fileName, string fileExtension,
			[Queue ("message-queue-avcontent")] out ContentEncodedMessage contentMessage, TraceWriter log)
		{
			log.Info ("Starting Function: EncodeAvContent");

			try
			{
				// check documentID before we take the time to encode
				if (!inputBlob.Metadata.TryGetValue (DocumentUpdatedMessage.DocumentIdKey, out string documentId) || string.IsNullOrWhiteSpace (documentId))
				{
					throw new Exception ($"inputBlob does not contain metadata item for {DocumentUpdatedMessage.DocumentIdKey}");
				}

				// check collectionId before we take the time to encode
				if (!inputBlob.Metadata.TryGetValue (DocumentUpdatedMessage.CollectionIdKey, out string collectionId) || string.IsNullOrWhiteSpace (collectionId))
				{
					throw new Exception ($"inputBlob does not contain metadata item for {DocumentUpdatedMessage.CollectionIdKey}");
				}


				_cachedCredentials = new MediaServicesCredentials (_mediaServicesAccountName, _mediaServicesAccountKey);

				_context = new CloudMediaContext (_cachedCredentials);


				var newAsset = CreateAssetFromBlob (inputBlob, log).GetAwaiter ().GetResult ();

				var newAssetName = $"{fileName}.{fileExtension} - {encoderProcessorName} encoded";


				var job = _context.Jobs.CreateWithSingleTask (encoderProcessorName, encoderTaskConfigName, newAsset, newAssetName, AssetCreationOptions.None);

				job.Submit ();


				log.Info ("Job Submit");


				job = job.StartExecutionProgressTask (
				   j => log.Info ($"Job ID:{job.Id} State: {job.State} Progress: {j.GetOverallProgress ().ToString ("0:0.##")}%"),
					CancellationToken.None
				).Result;


				switch (job.State)
				{
					case JobState.Finished:
						log.Info ($"Job {job.Id} is complete.");
						break;
					case JobState.Error: throw new Exception ("Job failed encoding.");
				}


				var outputAsset = job.OutputMediaAssets [0];

				_context.Locators.Create (LocatorType.OnDemandOrigin, outputAsset, AccessPermissions.Read, TimeSpan.FromDays (365 * 10), DateTime.UtcNow);



				var hlsUri = outputAsset.GetHlsUri ();


				log.Info ($"Output Asset - documentId: {documentId}, collectionId: {collectionId}, hlsUri: {hlsUri}");

				contentMessage = new ContentEncodedMessage (documentId, collectionId, hlsUri);

			}
			catch (Exception ex)
			{
				log.Error ($"ERROR: failed with exception {ex.Message}.\n {ex.StackTrace}");
				throw;
			}
		}


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
			_storageAccount = CloudStorageAccount.Parse (_storageAccountConnection);

			var blobClient = _storageAccount.CreateCloudBlobClient ();

			var writePolicy = _context.AccessPolicies.Create ("writePolicy", TimeSpan.FromHours (24), AccessPermissions.Write);


			var asset = _context.Assets.Create (blob.Name, AssetCreationOptions.None);


			var assetLocator = _context.Locators.CreateLocator (LocatorType.Sas, asset, writePolicy);

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