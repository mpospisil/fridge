using FridgeApp.Services;
using System.Threading.Tasks;
using FridgeApp.Tools;
using System;
using Xamarin.Forms;

namespace FridgeApp.ViewModels
{
	public interface IUserViewModel
	{
		string Name { get; set; }
		string Email { get; set; }
		void OnAppearing();
	}

	public class UserViewModel : BaseViewModel, IUserViewModel
	{
		private readonly IUserInfoService userInfoService;
		private string name;
		private string email;

		public UserViewModel(IFridgeDAL fridgeDal, IUserInfoService userInfoService) : base(fridgeDal)
		{
			this.userInfoService = userInfoService;
			Title = Resources.UserSettings;
			GetUserCommand = new Command(async () => await ExecuteGetUserCommand());
		}

		public string Name
		{
			get => name;
			set
			{
				SetProperty(ref name, value);
			}
		}

		public string Email
		{
			get => email;
			set
			{
				SetProperty(ref email, value);
			}
		}

		public Command GetUserCommand { get; }

		public void OnAppearing()
		{
			IsBusy = true;
		}

		public async Task ExecuteGetUserCommand()
		{
			var user = await FridgeDal.GetUserAsync();
			if(user == null)
			{
				// create a new user if he is missing
				var deviceUserData = await userInfoService.GetUserDataAsync();
				user = new Fridge.Model.User();
				user.Set(deviceUserData);
				user.UserId = Guid.NewGuid();
				await FridgeDal.CreateUserAsync(user);
			}

			Name = user.Name;
			Email = user.Email;

			IsBusy = false;
		}
	}
}
