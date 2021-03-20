using Fridge.Model;
using FridgeApp.DataContract;

namespace FridgeApp.Tools
{
	public static class UserTools
	{
		/// <summary>
		/// Copy param of <paramref name="src"/> to instance <paramref name="src"/>
		/// </summary>
		/// <param name="user"></param>
		/// <param name="src"></param>
		public static void Set(this User user, DeviceUserDetails src)
		{
			user.Email = src.Email;
			user.Name = src.Name;
		}
	}
}
