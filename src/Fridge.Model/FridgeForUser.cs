using System;

namespace Fridge.Model
{
	/// <summary>
	/// Information about user's fridge
	/// </summary>
	public class FridgeForUser
	{
		/// <summary>
		/// The unique identifier of the fridge
		/// </summary>
		public Guid FridgeId { get; set; }

		/// <summary>
		/// Id of the user who owns this fridge
		/// </summary>
		public Guid OwnerId { get; set; }

		/// <summary>
		/// My permission to this fridge
		/// </summary>
		public UserPermissionTypes MyPermission { get; set; }
	}
}
