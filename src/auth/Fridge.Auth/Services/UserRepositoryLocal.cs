using IdentityModel;
using IdentityServer4;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace Fridge.Auth.Services
{
	public class UserRepositoryLocal : IUserRepository
	{
		private List<IdentityUser> Users { get; set; }


		public UserRepositoryLocal()
		{
			Users = GetTestUsers();
		}
    /// <summary>
    /// Validates the credentials.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="password">The password.</param>
    /// <returns></returns>
    public bool ValidateCredentials(string username, string password)
    {
      var user = FindByUsername(username);

      if (user != null)
      {
        if (string.IsNullOrWhiteSpace(user.Password) && string.IsNullOrWhiteSpace(password))
        {
          return true;
        }

        return user.Password.Equals(password);
      }

      return false;
    }

    /// <summary>
    /// Finds the user by subject identifier.
    /// </summary>
    /// <param name="subjectId">The subject identifier.</param>
    /// <returns></returns>
    public IIdentityUser FindBySubjectId(string subjectId)
    {
      return Users.FirstOrDefault(x => x.SubjectId == subjectId);
    }

    /// <summary>
    /// Finds the user by username.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <returns></returns>
    public IIdentityUser FindByUsername(string username)
    {
      return Users.FirstOrDefault(x => x.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Finds the user by external provider.
    /// </summary>
    /// <param name="provider">The provider.</param>
    /// <param name="userId">The user identifier.</param>
    /// <returns></returns>
    public IIdentityUser FindByExternalProvider(string provider, string userId)
    {
      return Users.FirstOrDefault(x =>
          x.ProviderName == provider &&
          x.ProviderSubjectId == userId);
    }

    /// <summary>
    /// Automatically provisions a user.
    /// </summary>
    /// <param name="provider">The provider.</param>
    /// <param name="userId">The user identifier.</param>
    /// <param name="claims">The claims.</param>
    /// <returns></returns>
    public IIdentityUser AutoProvisionUser(string provider, string userId, List<Claim> claims)
    {
      // create a list of claims that we want to transfer into our store
      var filtered = new List<Claim>();

      foreach (var claim in claims)
      {
        // if the external system sends a display name - translate that to the standard OIDC name claim
        if (claim.Type == ClaimTypes.Name)
        {
          filtered.Add(new Claim(JwtClaimTypes.Name, claim.Value));
        }
        // if the JWT handler has an outbound mapping to an OIDC claim use that
        else if (JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.ContainsKey(claim.Type))
        {
          filtered.Add(new Claim(JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap[claim.Type], claim.Value));
        }
        // copy the claim as-is
        else
        {
          filtered.Add(claim);
        }
      }

      // if no display name was provided, try to construct by first and/or last name
      if (!filtered.Any(x => x.Type == JwtClaimTypes.Name))
      {
        var first = filtered.FirstOrDefault(x => x.Type == JwtClaimTypes.GivenName)?.Value;
        var last = filtered.FirstOrDefault(x => x.Type == JwtClaimTypes.FamilyName)?.Value;
        if (first != null && last != null)
        {
          filtered.Add(new Claim(JwtClaimTypes.Name, first + " " + last));
        }
        else if (first != null)
        {
          filtered.Add(new Claim(JwtClaimTypes.Name, first));
        }
        else if (last != null)
        {
          filtered.Add(new Claim(JwtClaimTypes.Name, last));
        }
      }

      // create a new unique subject id
      var sub = CryptoRandom.CreateUniqueId(format: CryptoRandom.OutputFormat.Hex);

      // check if a display name is available, otherwise fallback to subject id
      var name = filtered.FirstOrDefault(c => c.Type == JwtClaimTypes.Name)?.Value ?? sub;

      // create new user
      var user = new IdentityUser
      {
        SubjectId = sub,
        Username = name,
        ProviderName = provider,
        ProviderSubjectId = userId,
        Claims = filtered
      };

      // add user to in-memory store
      Users.Add(user);

      return user;
    }
    private static List<IdentityUser> GetTestUsers()
		{
			var address = new
			{
				street_address = "One Hacker Way",
				locality = "Heidelberg",
				postal_code = 69118,
				country = "Germany"
			};

			var tesUsers = new List<IdentityUser>
								{
										new IdentityUser
										{
												SubjectId = "818727",
												Username = "alice",
												Password = "alice",
												Claims =
												{
														new Claim(JwtClaimTypes.Name, "Alice Smith"),
														new Claim(JwtClaimTypes.GivenName, "Alice"),
														new Claim(JwtClaimTypes.FamilyName, "Smith"),
														new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
														new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
														new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
														new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address), IdentityServerConstants.ClaimValueTypes.Json)
												}
										},
										new IdentityUser
										{
												SubjectId = "88421113",
												Username = "bob",
												Password = "bob",
												Claims =
												{
														new Claim(JwtClaimTypes.Name, "Bob Smith"),
														new Claim(JwtClaimTypes.GivenName, "Bob"),
														new Claim(JwtClaimTypes.FamilyName, "Smith"),
														new Claim(JwtClaimTypes.Email, "BobSmith@email.com"),
														new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
														new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
														new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address), IdentityServerConstants.ClaimValueTypes.Json)
												}
										}
								};
			return tesUsers;
		}

	}
}
