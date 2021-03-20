using FridgeApp.Services;

namespace FridgeApp.DataContract
{
	/// <summary>
	/// Details about the user of the device
	/// </summary>
	public class DeviceUserDetails
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public DeviceUserDetails()
		{
			Name = MockFridgeDAL.User1Name;
			Email = MockFridgeDAL.User1Email;
		}

		/// <summary>
		/// Name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Email
		/// </summary>
		public string Email { get; set; }
	}
}
