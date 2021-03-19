using System;
using System.Collections.Generic;

namespace Fridge.Model
{
	/// <summary>
	/// Data about the user of the application
	/// </summary>
	public class User
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public User()
		{
			MyFridges = new List<FridgeForUser>();
		}

		/// <summary>
		/// The unique identifier of the user
		/// </summary>
		public Guid UserId { get; set; }

		/// <summary>
		/// Name of the user
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Email of the user
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		/// Fridges which are available for the user
		/// </summary>
		public List<FridgeForUser> MyFridges { get; set; }
	}
}
