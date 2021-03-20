using FridgeApp.DataContract;
using FridgeApp.Services;
using System.Threading.Tasks;

namespace FridgeApp.Droid
{
	public class UserDetailsProvider : IUserInfoService
	{
		public async Task<DeviceUserDetails> GetUserDataAsync()
		{
			var userDetails = new DeviceUserDetails();
			return await Task.FromResult(userDetails);
		}
	}
}