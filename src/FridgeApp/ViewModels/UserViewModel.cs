using FridgeApp.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace FridgeApp.ViewModels
{
	public interface IUserViewModel
	{

	}
	public class UserViewModel : BaseViewModel, IUserViewModel
	{
		public UserViewModel(IFridgeDAL fridgeDal) : base(fridgeDal)
		{
			Title = Resources.UserSettings;
		}
	}
}
