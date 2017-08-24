using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;

using Producer.Domain;

namespace Producer.Auth
{
	public static class SecurityExtensions
	{
		const string zumoAuthHeaderKey = "x-zumo-auth";
		const string JwtRegisteredClaimNamesIss = "iss";

		// https://github.com/Azure/azure-mobile-apps-net-server/wiki/Understanding-User-Ids
		public static string UniqueIdentifier (this IPrincipal user)
		{
			if (user is ClaimsPrincipal principal)
			{
				if (principal.Identity is ClaimsIdentity identity)
				{
					return identity.UniqueIdentifier ();
				}
			}

			return null;
		}


		public static string UniqueIdentifier (this ClaimsIdentity identity)
		{
			if (identity != null)
			{
				var stableSid = string.Empty;

				var ver = identity.FindFirst ("ver")?.Value;

				// the NameIdentifier claim is not stable.
				if (string.Compare (ver, "3", StringComparison.OrdinalIgnoreCase) == 0)
				{
					// the NameIdentifier claim is not stable.
					stableSid = identity.FindFirst ("stable_sid")?.Value;
				}
				else if (string.Compare (ver, "4", StringComparison.OrdinalIgnoreCase) == 0)
				{
					// the NameIdentifier claim is stable.
					stableSid = identity.FindFirst (ClaimTypes.NameIdentifier)?.Value;
				}

				var provider = identity.FindFirst ("http://schemas.microsoft.com/identity/claims/identityprovider")?.Value;

				if (string.IsNullOrEmpty (stableSid) || string.IsNullOrEmpty (provider))
				{
					return null;
				}

				return $"{provider}|{stableSid}";
			}

			return null;
		}


		public static Uri UriFromIssuerClaim (this ClaimsIdentity identity)
		{
			return new Uri (identity?.FindFirst (JwtRegisteredClaimNamesIss)?.Value);
		}


		public static void ConfigureClientForUserDetails (this HttpClient client, HttpRequestMessage req)
		{
			var zumoAuthHeader = req.Headers.GetValues (zumoAuthHeaderKey).FirstOrDefault ();

			client.DefaultRequestHeaders.Remove (zumoAuthHeaderKey);

			client.DefaultRequestHeaders.Add (zumoAuthHeaderKey, zumoAuthHeader);
		}


		public static ClaimsIdentity GetClaimsIdentity (this IPrincipal currentPricipal)
		{
			if (currentPricipal.Identity.IsAuthenticated
				&& currentPricipal is ClaimsPrincipal principal
				&& principal.Identity is ClaimsIdentity identity)
			{
				return identity;
			}

			return null;

		}

		public static UserRoles GetUserRole (this IPrincipal currentPricipal)
		{
			var identity = currentPricipal.GetClaimsIdentity ();

			if (identity != null)
			{
				return GetUserRole (identity);
			}

			return UserRoles.General;
		}


		public static UserRoles GetUserRole (this ClaimsIdentity identity)
		{
			if (identity.HasClaim (ClaimTypes.Role, UserRoles.Admin.Claim ()))
			{
				return UserRoles.Admin;
			}

			if (identity.HasClaim (ClaimTypes.Role, UserRoles.Producer.Claim ()))
			{
				return UserRoles.Producer;
			}

			if (identity.HasClaim (ClaimTypes.Role, UserRoles.Insider.Claim ()))
			{
				return UserRoles.Insider;
			}

			return UserRoles.General;
		}


		public static (string Id, UserRoles Role) GetUserIdAndRole (this IPrincipal currentPricipal)
		{
			var identity = currentPricipal.GetClaimsIdentity ();

			if (identity != null)
			{
				return (identity.UniqueIdentifier (), identity.GetUserRole ());
			}

			return (null, UserRoles.General);
		}

		public static bool HasId (this (string Id, UserRoles Role) user) => !string.IsNullOrEmpty (user.Id);

		public static bool CanWrite (this (string Id, UserRoles Role) user) => user.HasId () && user.Role.CanWrite ();
	}
}
