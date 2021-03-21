using Fridge.Model;
using FridgeApp.Services;
using System;
using System.Threading.Tasks;
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
		private string name;
		private string email;

		public UserViewModel(IFridgeDAL fridgeDal) : base(fridgeDal)
		{
			Title = Resources.UserSettings;
			GetUserCommand = new Command(async () => await ExecuteGetUserCommand());
			SaveCommand = new Command(OnSave, ValidateSave);
		}

		public Command SaveCommand { get; }

		private bool ValidateSave(object arg)
		{
			return true;
		}

		private void OnSave(object obj)
		{

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
			User user = await FridgeDal.GetUserAsync();
			if(user == null)
			{
				// create a new user if he is missing
				user = new User();
				user.Name = string.Empty;
				user.Email = string.Empty;
				user.UserId = Guid.Empty;
			}

			Name = user.Name;
			Email = user.Email;

			IsBusy = false;
		}
	}
}
