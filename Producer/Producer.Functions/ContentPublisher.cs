using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

using Producer.Domain;
using Producer.Auth;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.NotificationHubs;
using Newtonsoft.Json;

namespace Producer.Functions
{
	public static class ContentPublisher
	{

		static readonly string _documentDbUri = Environment.GetEnvironmentVariable ("RemoteDocumentDbUrl");
		static readonly string _documentDbKey = Environment.GetEnvironmentVariable ("RemoteDocumentDbKey");

		static DocumentClient _documentClient;
		static DocumentClient DocumentClient => _documentClient ?? (_documentClient = new DocumentClient (new Uri ($"https://{_documentDbUri}/"), _documentDbKey));


		[Authorize]
		[FunctionName ("ContentPublisher")]
		public static async Task<HttpResponseMessage> Run (
			[HttpTrigger (AuthorizationLevel.Anonymous, "post", Route = "publish")]HttpRequestMessage req,
			[NotificationHub (ConnectionStringSetting = "AzureNotificationHubConnection", HubName = "producer", Platform = NotificationPlatform.Apns, TagExpression = "(userrole:0 || userrole:{publishedTo})")] IAsyncCollector<Notification> notification,
			TraceWriter log)
		{
			log.Info ("new DocumentUpdatedMessage");

			UserStore userStore = null;

			var userId = Thread.CurrentPrincipal.GetClaimsIdentity ()?.UniqueIdentifier ();

			if (!string.IsNullOrEmpty (userId))
			{
				log.Info ($"User is authenticated and has userId: {userId}");

				userStore = await DocumentClient.GetUserStore (userId, log);
			}


			if (!userStore?.UserRole.CanWrite () ?? false)
			{
				log.Info ("Not authenticated");

				return req.CreateResponse (HttpStatusCode.Unauthorized);
			}


			try
			{
				var json = await req.Content.ReadAsStringAsync ();

				var updateMessage = JsonConvert.DeserializeObject<DocumentUpdatedMessage> (json);


				if (string.IsNullOrEmpty (updateMessage?.CollectionId))
				{
					throw new ArgumentException ("Must have value set for CollectionId", nameof (updateMessage));
				}


				var payload = ApsPayload.Create (updateMessage.Title, updateMessage.Message, updateMessage.CollectionId).Serialize ();


				log.Info ($"Sending Notification payload: {payload}");

				await notification.AddAsync (new AppleNotification (payload));

				return req.CreateResponse (HttpStatusCode.Accepted);
			}
			catch (Exception ex)
			{
				log.Error (ex.Message);
				throw;
			}
		}
	}
}
