// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace Fridge.Auth
{
	public static class Config
	{
		static string[] allowedScopes =
		{
						IdentityServerConstants.StandardScopes.OpenId,
						IdentityServerConstants.StandardScopes.Profile,
						IdentityServerConstants.StandardScopes.Email,
						//"resource1.scope1",
						//"resource2.scope1",
						//"transaction"
				};

		public static IEnumerable<IdentityResource> IdentityResources =>
				new IdentityResource[]
				{
				new IdentityResources.OpenId(),
				new IdentityResources.Profile(),
				new IdentityResources.Email()
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
		ClientName = "MVC Client",
		ClientUri = "https://localhost:5101",

		AllowedGrantTypes = GrantTypes.Hybrid,
		AllowOfflineAccess = true,
		ClientSecrets = { new Secret("secret".Sha256()) },

		RedirectUris =           { "https://localhost:5101/signin-oidc" },
		PostLogoutRedirectUris = { "https://localhost:5101/" },
		//LogoutUri =                "https://localhost:5101/signout-oidc",

		AllowedScopes =
		{
				IdentityServerConstants.StandardScopes.OpenId,
				IdentityServerConstants.StandardScopes.Profile,
				IdentityServerConstants.StandardScopes.Email,

				"api1",
		},
},

                ///////////////////////////////////////////
                // MVC Hybrid Flow Sample (Back Channel logout)
                //////////////////////////////////////////
                new Client
								{
										ClientId = "mvc.hybrid.backchannel",
										ClientName = "MVC Hybrid (with BackChannel logout)",
										ClientUri = "http://identityserver.io",

										ClientSecrets =
										{
												new Secret("secret".Sha256())
										},

										AllowedGrantTypes = GrantTypes.Hybrid,
										RequirePkce = false,

										RedirectUris = { "https://localhost:5101/signin-oidc" },
										BackChannelLogoutUri = "https://localhost:5101/logout",
										PostLogoutRedirectUris = { "https://localhost:5101/signout-callback-oidc" },

										AllowOfflineAccess = true,

										AllowedScopes = allowedScopes
								},

				new Client
				{
					ClientId = "xamarin-client",
					ClientName = "Xamarin Client",
					AllowedGrantTypes = GrantTypes.Code,
					AllowedScopes = new List<string>
					{
						IdentityServerConstants.StandardScopes.OpenId,
						IdentityServerConstants.StandardScopes.Profile,
						IdentityServerConstants.StandardScopes.Email,
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