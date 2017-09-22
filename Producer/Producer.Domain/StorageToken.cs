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

		public override string ToString ()
		{
			var sb = new System.Text.StringBuilder ("\n\nStorageToken\n");
			sb.Append ("  DocumentName".PadRight (13));
			sb.Append ($"{DocumentName}\n");
			sb.Append ("  SasUri".PadRight (13));
			sb.Append ($"{SasUri}\n");
			return sb.ToString ();
		}
	}
}
