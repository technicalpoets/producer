using System;

using Microsoft.WindowsAzure.Storage.Blob;

using Producer.Domain;

namespace Producer.Functions
{
	public static class FunctionExtensions
	{
		public static void HasValueOrThrow (params (string Name, string Value) [] parameters)
		{
			foreach (var parameter in parameters)
			{
				if (string.IsNullOrEmpty (parameter.Value))
				{
					throw new ArgumentNullException (parameter.Name);
				}
			}
		}

		public static void HasValueOrThrow (string parameter, string parameterName, string errorMessage = null)
		{
			if (string.IsNullOrEmpty (parameter))
			{
				throw new ArgumentNullException (parameterName, errorMessage ?? $"{parameterName} must have value.");
			}
		}


		public static ContentEncodedMessage GetMessageOrThrow (CloudBlockBlob inputBlob)
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

			return new ContentEncodedMessage (documentId, collectionId);
		}
	}
}
