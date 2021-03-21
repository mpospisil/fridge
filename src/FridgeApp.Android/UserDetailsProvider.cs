using Android.Accounts;
using FridgeApp.DataContract;
using FridgeApp.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms.Platform.Android;
using System.Linq;

namespace FridgeApp.Droid
{
	public class UserDetailsProvider : IUserInfoService
	{
    
    public async Task<DeviceUserDetails> GetUserDataAsync()
		{
			var userDetails = new DeviceUserDetails();

			AccountManager manager = AccountManager.Get(Android.App.Application.Context);
			Account[] accounts = manager.GetAccountsByType("com.google");
			if(accounts.Any())
			{
				var firstAccount = accounts.First();
				var name = firstAccount.Name;
			}

			return await Task.FromResult(userDetails);
		}
  }
}