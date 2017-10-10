using System.Collections.Generic;

namespace Producer.Domain
{
	public class PushTemplate
	{
		public static string iOS = "{'aps':{'alert':{'title':'$(Title)','body':'$(Body)'},'content-available':#(ContentAvailable)},'collectionId':'$(CollectionId)'}";
		public static string Android = "{'notification':{'title':'$(Title)','body':'$(Body)'},'data':{'content-available':#(ContentAvailable),'collectionId':'$(CollectionId)'}";

		public string Title { get; set; }
		public string Body { get; set; }
		public string ContentAvailable { get; set; }
		public string CollectionId { get; set; }

		public Dictionary<string, string> GetProperties ()
		{
			return new Dictionary<string, string>
			{
				{ nameof (Title), Title },
				{ nameof (Body), Body },
				{ nameof (ContentAvailable), ContentAvailable },
				{ nameof (CollectionId), CollectionId }
			};
		}

		public static PushTemplate FromMessage (DocumentUpdatedMessage message)
		{
			return new PushTemplate
			{
				Title = string.IsNullOrWhiteSpace (message?.Title) ? null : message.Title,
				Body = string.IsNullOrWhiteSpace (message?.Message) ? null : message.Message,
				ContentAvailable = string.IsNullOrWhiteSpace (message?.CollectionId) ? "0" : "1",
				CollectionId = string.IsNullOrWhiteSpace (message?.CollectionId) ? null : message.CollectionId
			};
		}
	}
}
