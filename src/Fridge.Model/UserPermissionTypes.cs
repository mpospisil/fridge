namespace Fridge.Model
{
	/// <summary>
	/// Types of users permissions
	/// </summary>
	public enum UserPermissionTypes
	{
		/// <summary>
		/// Full access. Owner can delete fridge
		/// </summary>
		Owner,

		/// <summary>
		/// User of the shared fridge
		/// </summary>
		SharedFridgeUser,
	}
}
