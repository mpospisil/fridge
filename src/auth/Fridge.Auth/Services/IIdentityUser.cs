using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Fridge.Auth.Services
{
	public interface IIdentityUser
	{
		//
		// Summary:
		//     Gets or sets the subject identifier.
		string SubjectId { get; set; }
		//
		// Summary:
		//     Gets or sets the username.
		string Username { get; set; }
		//
		// Summary:
		//     Gets or sets the password.
		string Password { get; set; }
		//
		// Summary:
		//     Gets or sets the provider name.
		string ProviderName { get; set; }
		//
		// Summary:
		//     Gets or sets the provider subject identifier.
		string ProviderSubjectId { get; set; }
		//
		// Summary:
		//     Gets or sets if the user is active.
		bool IsActive { get; set; }
		//
		// Summary:
		//     Gets or sets the claims.
		public ICollection<Claim> Claims { get; set; }
	}
}
