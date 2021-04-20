using Fridge.Model;
using System.Collections.Generic;
using System.Security.Claims;

namespace Fridge.Auth.Services
{
	public interface IUserRepository
	{
		public IIdentityUser AutoProvisionUser(string provider, string userId, List<Claim> claims);

		public IIdentityUser FindByExternalProvider(string provider, string userId);

		public IIdentityUser FindBySubjectId(string subjectId);

		public IIdentityUser FindByUsername(string username);

		public bool ValidateCredentials(string username, string password);
	}
}
