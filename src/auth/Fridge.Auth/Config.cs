// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace Fridge.Auth
{
	public static class Config
	{
		public static IEnumerable<IdentityResource> IdentityResources =>
				new IdentityResource[]
				{
				new IdentityResources.OpenId(),
				new IdentityResources.Profile(),
				};

		public static IEnumerable<ApiScope> ApiScopes =>
				new List<ApiScope>
				{
						new ApiScope("api1", "My API")
				};

		public static IEnumerable<Client> Clients =>
				new List<Client>
				{
				new Client
				{
						ClientId = "client",

            // no interactive user, use the clientid/secret for authentication
            AllowedGrantTypes = GrantTypes.ClientCredentials,

            // secret for authentication
            ClientSecrets =
						{
								new Secret("secret".Sha256())
						},

            // scopes that client has access to
            AllowedScopes = { "api1" }
				},

				new Client
				{
						ClientId = "mvc",
						ClientSecrets = { new Secret("secret".Sha256()) },

						AllowedGrantTypes = GrantTypes.Code,

            // where to redirect to after login
            RedirectUris = { "https://localhost:5101/signin-oidc" },

            // where to redirect to after logout
            PostLogoutRedirectUris = { "https://localhost:5101/signout-callback-oidc" },

						AllowedScopes = new List<string>
						{
								IdentityServerConstants.StandardScopes.OpenId,
								IdentityServerConstants.StandardScopes.Profile
						}
				},

				new Client
				{
					ClientId = "xamarin-client",
					ClientName = "Xamarin Client",
					AllowedGrantTypes = GrantTypes.Code,
					AllowedScopes = new List<string>
					{
						IdentityServerConstants.StandardScopes.OpenId,
						IdentityServerConstants.StandardScopes.Profile
					},
					AllowAccessTokensViaBrowser = true,
					AllowOfflineAccess = true,
					AlwaysIncludeUserClaimsInIdToken = true,
					RequirePkce = true,
					RequireClientSecret = false,
					RedirectUris = { "https://localhost:5001/grands"},
				},

								new Client
								{
										ClientId = "interactive.public",
										ClientName = "Interactive client (Code with PKCE)",

										RedirectUris = { "https://notused" },
										PostLogoutRedirectUris = { "https://notused" },

										RequireClientSecret = false,

										AllowedGrantTypes = GrantTypes.Code,
										AllowedScopes = { "openid", "profile", "email"},

										AllowOfflineAccess = true,
										RefreshTokenUsage = TokenUsage.OneTimeOnly,
										RefreshTokenExpiration = TokenExpiration.Sliding
								},
				};
	}
}