using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

using Producer.Auth;
using Producer.Domain;

namespace Producer.Functions
{
	public static class PublishContent
	{

		static DocumentClient _documentClient;
		static DocumentClient DocumentClient => _documentClient ?? (_documentClient = new DocumentClient (EnvironmentVariables.DocumentDbUri, EnvironmentVariables.DocumentDbKey));


		[Authorize]
		[FunctionName (nameof (PublishContent))]
		public static async Task<HttpResponseMessage> Run (
			[HttpTrigger (AuthorizationLevel.Anonymous, Routes.Post, Route = Routes.PublishContent)] DocumentUpdatedMessage updateMessage,
			[NotificationHub (ConnectionStringSetting = EnvironmentVariables.AzureWebJobsNotificationHubsConnectionString, /*Platform = NotificationPlatform.Apns,*/ TagExpression = "{NotificationTags}")] IAsyncCollector<Notification> notification,
			TraceWriter log)
		{
			log.Info (updateMessage?.ToString ());

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

				throw new HttpResponseException (HttpStatusCode.Unauthorized);
			}

			try
			{
				FunctionExtensions.HasValueOrThrow (updateMessage?.CollectionId, DocumentUpdatedMessage.CollectionIdKey);

				var template = PushTemplate.FromMessage (updateMessage);

				await notification.AddAsync (new TemplateNotification (template.GetProperties ()));

				throw new HttpResponseException (HttpStatusCode.Accepted);
			}
			catch (HttpResponseException response)
			{
				return response.Response;
			}
			catch (Exception ex)
			{
				log.Error (ex.Message, ex);
				throw;
			}
		}
	}
}
