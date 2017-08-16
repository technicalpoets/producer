using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

using Producer.Domain;

namespace Producer.Functions
{
	public static class StorageTokenGenerator
	{
		static readonly string _storageAccountConnection = Environment.GetEnvironmentVariable ("AzureWebJobsStorage");

		static CloudBlobClient _blobClient;

		public static CloudBlobClient BlobClient => _blobClient ?? (_blobClient = CloudStorageAccount.Parse (_storageAccountConnection).CreateCloudBlobClient ());

#if !DEBUG
		//[Authorize]
#endif
		[FunctionName ("GetStorageToken")]
		public static async Task<HttpResponseMessage> Run (
			[HttpTrigger (AuthorizationLevel.Anonymous, "get", Route = "tokens/{collectionId}/{documentId}")] HttpRequestMessage req,
			[DocumentDB ("Content", "{collectionId}", Id = "{documentId}")] Content content,
			string collectionId, string documentId, TraceWriter log)
		{
#if !DEBUG
			//if (!Thread.CurrentPrincipal.Identity.IsAuthenticated)
			//{
			//	log.Info ("Not authenticated");

			//	return req.CreateResponse (HttpStatusCode.Unauthorized);
			//}
#endif

			if (content == null)
			{
				log.Info ($"No item in database matching the documentId paramater {documentId}");

				return req.CreateErrorResponse (HttpStatusCode.NotFound, $"No item in database matching the documentId paramater {documentId}");
			}

			log.Info ($"Successfully found document in database matching the documentId paramater {documentId}");


			var containerName = $"uploads-{collectionId.ToLower ()}"; // Example: uploads-avcontent


			// Errors creating the storage container result in a 500 Internal Server Error
			var container = BlobClient.GetContainerReference (containerName);

			await container.CreateIfNotExistsAsync ();


			// TODO: Check if there's already a blob with a name matching the Id


			var sasUri = getBlobSasUri (container, content.Name);

			var token = new StorageToken (content.Name, sasUri);

			return req.CreateResponse (HttpStatusCode.OK, token);
		}


		static string getBlobSasUri (CloudBlobContainer container, string blobName, string policyName = null)
		{
			string sasBlobToken;

			// Get a reference to a blob within the container.
			// Note that the blob may not exist yet, but a SAS can still be created for it.
			CloudBlockBlob blob = container.GetBlockBlobReference (blobName);

			if (policyName == null)
			{
				// Create a new access policy and define its constraints.
				// Note that the SharedAccessBlobPolicy class is used both to define the parameters of an ad-hoc SAS, and
				// to construct a shared access policy that is saved to the container's shared access policies.
				SharedAccessBlobPolicy adHocSAS = new SharedAccessBlobPolicy ()
				{
					// When the start time for the SAS is omitted, the start time is assumed to be the time when the storage service receives the request.
					// Omitting the start time for a SAS that is effective immediately helps to avoid clock skew.
					SharedAccessExpiryTime = DateTime.UtcNow.AddHours (24),
					Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.Create
				};

				// Generate the shared access signature on the blob, setting the constraints directly on the signature.
				sasBlobToken = blob.GetSharedAccessSignature (adHocSAS);
			}
			else
			{
				// Generate the shared access signature on the blob. In this case, all of the constraints for the
				// shared access signature are specified on the container's stored access policy.
				sasBlobToken = blob.GetSharedAccessSignature (null, policyName);
			}

			// Return the URI string for the container, including the SAS token.
			return blob.Uri + sasBlobToken;
		}
	}
}
