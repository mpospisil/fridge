using System.Collections.Generic;
using System.Security.Claims;

namespace Fridge.Model
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
		ICollection<Claim> Claims { get; set; }
	}
}
