using System;
using System.Net;
using System.Net.Http;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

using Producer.Domain;

namespace Producer.Functions
{
	public static class ContentPublisher
	{

#if !DEBUG
		[Authorize]
#endif
		[FunctionName ("ContentPublisher")]
		public static bool Run (
			[HttpTrigger (AuthorizationLevel.Anonymous, "post", Route = "publish")]DocumentUpdatedMessage req,
			//[DocumentDB ("Content", "{collectionId}", Id = "{documentId}")] Content document,
			//string collectionId,
			//string documentId,
			//int publishTo,
			[Queue ("message-queue-document-update")] out DocumentUpdatedMessage updatedMessage,
			TraceWriter log)
		{
#if !DEBUG
			if (!Thread.CurrentPrincipal.Identity.IsAuthenticated)
			{
				log.Info ("Not authenticated");

				return req.CreateResponse (HttpStatusCode.Unauthorized);
			}
#endif

			log.Info ("new DocumentUpdatedMessage");
			log.Info (Newtonsoft.Json.JsonConvert.SerializeObject (req));

			updatedMessage = req;

			return true;

			//if (document != null)
			//{
			//	try
			//	{
			//		document.PublishedTo = (UserRoles) publishTo;
			//		document.PublishedAt = DateTimeOffset.Now;


			//		log.Info ($"Successfully found document in database matching the documentId paramater {documentId}");

			//		updatedMessage = req
			//			new DocumentUpdatedMessage (document.Id, collectionId);

			//		updatedMessage.Title = document.DisplayName;

			//		updatedMessage.Message = "New Content!";

			//		return req.CreateResponse (HttpStatusCode.OK);

			//	}
			//	catch (Exception ex)
			//	{
			//		log.Error (ex.Message);
			//		throw;
			//	}
			//}
			//else
			//{
			//	//return req.CreateResponse (HttpStatusCode.NotFound);
			//	var ex = new Exception ($"Unable to find record with Id {documentId}");
			//	log.Error (ex.Message);
			//	throw ex;
			//}
		}
	}
}
