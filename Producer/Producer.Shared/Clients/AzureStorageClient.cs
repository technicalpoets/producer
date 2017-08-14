using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage.Blob;

using Producer.Domain;


namespace Producer.Shared
{
	public class AzureStorageClient
	{

		static AzureStorageClient _shared;
		public static AzureStorageClient Shared => _shared ?? (_shared = new AzureStorageClient ());


		public async Task<bool> AddNewFileAsync (AvContent avContent, StorageToken storageToken)
		{
			try
			{
				var sasUri = new Uri (storageToken.SasUri);

				var blockBlob = new CloudBlockBlob (sasUri);

				blockBlob.Metadata [StorageToken.ContentIdParam] = avContent.Id;

				blockBlob.Metadata ["displayName"] = avContent.Name.SplitOnLast ('.').FirstOrDefault () ?? avContent.Name;

				await blockBlob.UploadFromFileAsync (avContent.LocalInboxPath);

				Log.Debug ($"Finished uploading new file.");

				return true;
			}
			catch (Exception ex)
			{
				Log.Debug ($"{ex.Message}");

				return false;
			}
		}
	}
}