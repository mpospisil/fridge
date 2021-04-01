// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Fridge.Auth
{
	public class Startup
	{
		public IWebHostEnvironment Environment { get; }

		public Startup(IWebHostEnvironment environment)
		{
			Environment = environment;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			// uncomment, if you want to add an MVC-based UI
			services.AddControllersWithViews();

			var builder = services.AddIdentityServer(options =>
			{
							// see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
							options.EmitStaticAudienceClaim = true;
			})
					.AddInMemoryIdentityResources(Config.IdentityResources)
					.AddInMemoryApiScopes(Config.ApiScopes)
					.AddInMemoryClients(Config.Clients);

			// not recommended for production - you need to store your key material somewhere secure
			builder.AddDeveloperSigningCredential();

			services.AddAuthentication()
					.AddGoogle(options =>
					{
						options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

						// register your IdentityServer with Google at https://console.developers.google.com
						// enable the Google+ API
						// set the redirect URI to https://localhost:5001/signin-google
						options.ClientId = "556744474248-2ovfttlln1uat9mmt05a4q45pj3ta4dq.apps.googleusercontent.com";
						options.ClientSecret = "qinqSz_deP6_mY3E7IJy-7bu";
					});
		}

		public void Configure(IApplicationBuilder app)
		{
			if (Environment.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			// uncomment if you want to add MVC
			app.UseStaticFiles();
			app.UseRouting();

			app.UseIdentityServer();

			// uncomment, if you want to add MVC
			app.UseAuthorization();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapDefaultControllerRoute();
			});
		}
	}
}
