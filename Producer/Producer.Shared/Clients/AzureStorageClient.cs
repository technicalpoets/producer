using System;
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
				Log.Debug (storageToken?.ToString ());

				var sasUri = new Uri (storageToken.SasUri);

				var blockBlob = new CloudBlockBlob (sasUri);

				blockBlob.Metadata [DocumentUpdatedMessage.DocumentIdKey] = avContent.Id;

				blockBlob.Metadata [DocumentUpdatedMessage.CollectionIdKey] = typeof (AvContent).Name;

				UpdateNetworkActivityIndicator (true);

				await blockBlob.UploadFromFileAsync (avContent.LocalInboxPath);

				Log.Debug ($"Finished uploading new file.");

				return true;
			}
			catch (Exception ex)
			{
				Log.Error (ex);

				return false;
			}
			finally
			{
				UpdateNetworkActivityIndicator (false);
			}
		}


		void UpdateNetworkActivityIndicator (bool visible)
		{
#if __IOS__
			UIKit.UIApplication.SharedApplication.BeginInvokeOnMainThread (() => UIKit.UIApplication.SharedApplication.NetworkActivityIndicatorVisible = visible);
#endif
		}
	}
}