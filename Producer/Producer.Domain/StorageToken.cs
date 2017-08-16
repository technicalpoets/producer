using System;

namespace Producer.Domain
{
	public class StorageToken
	{
		public string DocumentName { get; private set; }

		public string SasUri { get; private set; }

		public StorageToken (string documentName, string sasUri)
		{
			DocumentName = documentName ?? throw new ArgumentNullException (nameof (documentName));
			SasUri = sasUri ?? throw new ArgumentNullException (nameof (sasUri));
		}
	}
}
