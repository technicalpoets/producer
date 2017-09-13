using Microsoft.Azure.Documents;

using HttpStatusCode = System.Net.HttpStatusCode;


namespace Producer.Domain
{
	public static class ExceptionExtensions
	{
#if __MOBILE__
		public static string Print (this DocumentClientException dex)
		{
			if ((int) dex.StatusCode == 429)
			{
				return "TooManyRequests - This means you have exceeded the number of request units per second. Consult the DocumentClientException.RetryAfter value to see how long you should wait before retrying this operation.";
			}
			else
			{
				switch (dex.StatusCode)
				{
					case HttpStatusCode.BadRequest:
						return "BadRequest - This means something was wrong with the document supplied. It is likely that disableAutomaticIdGeneration was true and an id was not supplied";
					case HttpStatusCode.Forbidden:
						return "Forbidden - This likely means the collection in to which you were trying to create the document is full.";
					case HttpStatusCode.Conflict:
						return "Conflict - This means a Document with an id matching the id field of document already existed";
					case HttpStatusCode.RequestEntityTooLarge:
						return "RequestEntityTooLarge - This means the Document exceeds the current max entity size. Consult documentation for limits and quotas.";
					default:
						return dex.Message;
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
					case HttpStatusCode.BadRequest:
						log?.Info ("BadRequest - This means something was wrong with the document supplied. It is likely that disableAutomaticIdGeneration was true and an id was not supplied");
						break;
					case HttpStatusCode.Forbidden:
						log?.Info ("Forbidden - This likely means the collection in to which you were trying to create the document is full.");
						break;
					case HttpStatusCode.Conflict:
						log?.Info ("Conflict - This means a Document with an id matching the id field of document already existed");
						break;
					case HttpStatusCode.RequestEntityTooLarge:
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
