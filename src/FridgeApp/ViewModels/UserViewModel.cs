using Fridge.Model;
using FridgeApp.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FridgeApp.ViewModels
{
	public interface IUserViewModel
	{
		Guid UserId { get; set; }
		string Name { get; set; }
		string Email { get; set; }
		void OnAppearing();
	}

	public class UserViewModel : BaseViewModel, IUserViewModel
	{
		private string name;
		private string email;
		private Guid userId;

		public UserViewModel(IFridgeDAL fridgeDal) : base(fridgeDal)
		{
			Title = Resources.UserSettings;
			GetUserCommand = new Command(async () => await ExecuteGetUserCommand());
			SaveCommand = new Command(async () => await OnSave(), ValidateSave);

			this.PropertyChanged +=
					(_, __) => SaveCommand.ChangeCanExecute();
		}

		public Command SaveCommand { get; }

		private bool ValidateSave()
		{
			if(String.IsNullOrWhiteSpace(Name))
			{
				return false;
			}

			if (String.IsNullOrWhiteSpace(Email))
			{
				return false;
			}

			{
				try
				{
					var emailString = Email;
					emailString.ToLower();
					var addr = new System.Net.Mail.MailAddress(emailString);
					return addr.Address == emailString;
				}
				catch
				{
					return false;
				}
			}
		}

		private async Task OnSave()
		{
			var isNewUser = await Save();
			if(isNewUser)
			{
				((AppShell)Shell.Current).OpenSettingsPage();
			}
			else
			{
				await Shell.Current.GoToAsync("..");
			}
		}

		public async Task<bool> Save()
		{
			if(UserId == Guid.Empty)
			{
				// this is the new user

				User newUser = new User();
				newUser.Name = Name;
				newUser.Email = Email;
				newUser.UserId = Guid.NewGuid();

				await FridgeDal.CreateUserAsync(newUser);

				return true;
			}
			else
			{
				// update the existing user
				return false;
			}
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

		public Guid UserId
		{
			get => userId;
			set
			{
				SetProperty(ref userId, value);
			}
		}
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

			UserId = user.UserId;
			Name = user.Name;
			Email = user.Email;

			IsBusy = false;
		}
	}
}
