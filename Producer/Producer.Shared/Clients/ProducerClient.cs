using System;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;

using SettingsStudio;

using Producer.Domain;
using System.Text;

namespace Producer.Shared
{
	public class ProducerClient
	{
		static ProducerClient _shared;
		public static ProducerClient Shared => _shared ?? (_shared = new ProducerClient ());


		HttpClient client;

		ProducerClient ()
		{
			client = new HttpClient ();
		}


		public async Task Publish<T> (T content, string notificationTitle = null, string notificationMessage = null)
			where T : Content
		{
			if (content?.HasId ?? false)
			{
				var url = $"{Settings.FunctionsUrl}/api/publish";

				try
				{
					var updateMessage = new DocumentUpdatedMessage (content.Id, typeof (T).Name)
					{
						Title = notificationTitle,
						Message = notificationMessage
					};

					var response = await client.PostAsync (url, new StringContent (JsonConvert.SerializeObject (updateMessage), Encoding.UTF8, "application/json"));

					Log.Debug (response.ToString ());

					//var stringContent = await response.Content.ReadAsStringAsync ();

					//return JsonConvert.DeserializeObject<StorageToken> (stringContent);
				}
				catch (Exception ex)
				{
					Log.Debug (ex.Message);
					throw;
				}
			}
		}


		public async Task<StorageToken> GetStorageToken<T> (T content)
			where T : Content
		{
			if (content?.HasId ?? false)
			{
				var url = $"{Settings.FunctionsUrl}/api/tokens/{typeof (T).Name}/{content.Id}";

				try
				{
					var response = await client.GetAsync (url);

					var stringContent = await response.Content.ReadAsStringAsync ();

					return JsonConvert.DeserializeObject<StorageToken> (stringContent);
				}
				catch (Exception ex)
				{
					Log.Debug (ex.Message);
					throw;
				}
			}

			return null;
		}
	}
}
