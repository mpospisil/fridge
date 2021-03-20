using FridgeApp.DataContract;
using System.Threading.Tasks;

namespace FridgeApp.Services
{
	public interface IUserInfoService
	{
			Task<DeviceUserDetails> GetUserDataAsync();
	}
}
