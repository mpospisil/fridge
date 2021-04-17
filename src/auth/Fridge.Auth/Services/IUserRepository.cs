using System.Collections.Generic;
using System.Security.Claims;

namespace Fridge.Auth.Services
{
	public interface IUserRepository<UserType>
	{
		public UserType AutoProvisionUser(string provider, string userId, List<Claim> claims);

		public UserType FindByExternalProvider(string provider, string userId);

		public UserType FindBySubjectId(string subjectId);

		public UserType FindByUsername(string username);

		public bool ValidateCredentials(string username, string password);
	}
}
