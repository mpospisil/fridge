using Fridge.Auth.Services;
using IdentityServer4;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Fridge.Auth
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllersWithViews();

			var builder = services.AddIdentityServer(options =>
			{
				// see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
				options.EmitStaticAudienceClaim = true;

			})
				.AddInMemoryIdentityResources(Config.IdentityResources)
				.AddInMemoryApiScopes(Config.ApiScopes)
				.AddSigningCredential(new Services.CertificateStore().GetCertificate())
				.AddInMemoryClients(Config.Clients)
				.AddProfileService<FridgeUserProfileService>()
				.Services.AddSingleton<IUserRepository, Fridge.Auth.Services.UserRepositoryLocal>();

			// not recommended for production - you need to store your key material somewhere secure
			//builder.AddDeveloperSigningCredential();

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

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
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
