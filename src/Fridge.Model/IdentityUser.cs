using System.Collections.Generic;
using System.Security.Claims;

namespace Fridge.Model
{
	public class IdentityUser : IIdentityUser
	{
		public IdentityUser()
		{
			Claims = new List<Claim>();
		}

		public string SubjectId { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public string ProviderName { get; set; }
		public string ProviderSubjectId { get; set; }
		public bool IsActive { get; set; }
		public ICollection<Claim> Claims { get; set; }
	}
}
