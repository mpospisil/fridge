using Fridge.Model;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Fridge.Auth.Services
{
	public class UserRepositoryLocal : IUserRepository<Fridge.Model.User>
	{
		public User AutoProvisionUser(string provider, string userId, List<Claim> claims)
		{
			throw new NotImplementedException();
		}

		public User FindByExternalProvider(string provider, string userId)
		{
			throw new NotImplementedException();
		}

		public User FindBySubjectId(string subjectId)
		{
			throw new NotImplementedException();
		}

		public User FindByUsername(string username)
		{
			throw new NotImplementedException();
		}

		public bool ValidateCredentials(string username, string password)
		{
			throw new NotImplementedException();
		}
	}
}
