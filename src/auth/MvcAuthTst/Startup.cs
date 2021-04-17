using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IdentityModel.Tokens.Jwt;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;

namespace MvcClient
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

			JwtSecurityTokenHandler.DefaultMapInboundClaims = false;


      services.AddAuthentication(options =>
      {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = "oidc";
      })
          .AddCookie(options =>
          {
            options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
            options.Cookie.Name = "mvchybridbc";

            //options.EventsType = typeof(CookieEventHandler);
          })
          .AddOpenIdConnect("oidc", options =>
          {
            options.Authority = "https://localhost:5001";
            options.RequireHttpsMetadata = false;

            options.ClientSecret = "secret";
            options.ClientId = "mvc.hybrid.backchannel";

            options.ResponseType = "code id_token";

            options.Scope.Clear();
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            options.Scope.Add("email");
            //options.Scope.Add("resource1.scope1");
            //options.Scope.Add("offline_access");

            //options.ClaimActions.MapAllExcept("iss", "nbf", "exp", "aud", "nonce", "iat", "c_hash");

            options.GetClaimsFromUserInfoEndpoint = true;
            options.SaveTokens = true;

            options.TokenValidationParameters = new TokenValidationParameters
            {
              NameClaimType = JwtClaimTypes.Name,
              RoleClaimType = JwtClaimTypes.Role,
            };
          });
    }

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseStaticFiles();

			app.UseRouting();
			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapDefaultControllerRoute();
						//.RequireAuthorization();
			});
		}
	}
}
