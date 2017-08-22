using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Producer.Auth
{
	public class AzureAppServiceUser
	{
		public const string AuthenticationHeader = "x-zumo-auth";

		[JsonProperty ("userId")]
		public string UserId { get; set; }

		[JsonProperty ("authenticationToken")]
		public string AuthenticationToken { get; set; }
	}
}