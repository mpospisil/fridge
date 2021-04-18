using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services; 
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Fridge.Auth.Services
{
	public class FridgeUserProfileService : IProfileService
	{
		IUserRepository Users { get; set; } 
		public FridgeUserProfileService(IUserRepository userRepos)
		{
			this.Users = userRepos;
		}


		public Task GetProfileDataAsync(ProfileDataRequestContext context)
		{
			if (context.RequestedClaimTypes.Any())
			{
				var user = Users.FindBySubjectId(context.Subject.GetSubjectId());
				if (user != null)
				{
					context.AddRequestedClaims(user.Claims);
				}
			}

			return Task.CompletedTask;

			//var claims = new List<Claim>
			//	{
			//										new Claim(JwtClaimTypes.Name, "Alice Smith"),
			//											new Claim(JwtClaimTypes.GivenName, "Alice"),
			//											new Claim(JwtClaimTypes.FamilyName, "Smith"),
			//											new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
			//											new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
			//											new Claim(JwtClaimTypes.WebSite, "http://alice.com"),

			//	};

			//context.IssuedClaims.AddRange(claims);

			//return Task.CompletedTask;
		}

		public Task IsActiveAsync(IsActiveContext context)
		{
			context.IsActive = true;
			return Task.CompletedTask;
		}
	}
}
