using Microsoft.Azure.Documents;

namespace Producer.Domain
{
	public static class ExceptionExtensions
	{
#if __MOBILE__
		public static void Print (this DocumentClientException dex)
		{
			if ((int) dex.StatusCode == 429)
			{
				Log.Debug ("TooManyRequests - This means you have exceeded the number of request units per second. Consult the DocumentClientException.RetryAfter value to see how long you should wait before retrying this operation.");
			}
			else
			{
				switch (dex.StatusCode)
				{
					case System.Net.HttpStatusCode.BadRequest:
						Log.Debug ("BadRequest - This means something was wrong with the document supplied. It is likely that disableAutomaticIdGeneration was true and an id was not supplied");
						break;
					case System.Net.HttpStatusCode.Forbidden:
						Log.Debug ("Forbidden - This likely means the collection in to which you were trying to create the document is full.");
						break;
					case System.Net.HttpStatusCode.Conflict:
						Log.Debug ("Conflict - This means a Document with an id matching the id field of document already existed");
						break;
					case System.Net.HttpStatusCode.RequestEntityTooLarge:
						Log.Debug ("RequestEntityTooLarge - This means the Document exceeds the current max entity size. Consult documentation for limits and quotas.");
						break;
					default:
						break;
				}
			}
		}
#else
		public static void Print (this DocumentClientException dex, Microsoft.Azure.WebJobs.Host.TraceWriter log)
		{
			if ((int) dex.StatusCode == 429)
			{
				log?.Info ("TooManyRequests - This means you have exceeded the number of request units per second. Consult the DocumentClientException.RetryAfter value to see how long you should wait before retrying this operation.");
			}
			else
			{
				switch (dex.StatusCode)
				{
					case System.Net.HttpStatusCode.BadRequest:
						log?.Info ("BadRequest - This means something was wrong with the document supplied. It is likely that disableAutomaticIdGeneration was true and an id was not supplied");
						break;
					case System.Net.HttpStatusCode.Forbidden:
						log?.Info ("Forbidden - This likely means the collection in to which you were trying to create the document is full.");
						break;
					case System.Net.HttpStatusCode.Conflict:
						log?.Info ("Conflict - This means a Document with an id matching the id field of document already existed");
						break;
					case System.Net.HttpStatusCode.RequestEntityTooLarge:
						log?.Info ("RequestEntityTooLarge - This means the Document exceeds the current max entity size. Consult documentation for limits and quotas.");
						break;
					default:
						break;
				}
			}
		}
#endif
	}
}
