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
		private readonly IFridgeLogger Logger;
		private string name;
		private string email;
		private Guid userId;

		public UserViewModel(IFridgeDAL fridgeDal, IFridgeLogger logger) : base(fridgeDal)
		{
			Logger = logger;
			Title = Resources.UserSettings;
			GetUserCommand = new Command(async () => await ExecuteGetUserCommand());
			SaveCommand = new Command(async () => await OnSave(), ValidateSave);
			ResetCommand = new Command(OnReset);

			this.PropertyChanged +=
					(_, __) => SaveCommand.ChangeCanExecute();

			this.PropertyChanged +=
					(_, __) => ResetCommand.ChangeCanExecute();
		}

		public Command SaveCommand { get; }
		public Command ResetCommand { get; }

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
				OnPropertyChanged("IsNewUser");
			}
		}

		public bool IsNewUser
		{
			get
			{
				return (UserId == Guid.Empty);
			}
		}

		public void OnAppearing()
		{
			IsBusy = true;
		}

		public async Task<bool> Save()
		{
			Logger.LogDebug("UserViewModel.Save");
			if (UserId == Guid.Empty)
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


		public async Task ExecuteGetUserCommand()
		{
			Logger.LogDebug("UserViewModel.ExecuteGetUserCommand");
			User user = await FridgeDal.GetUserAsync();
			if (user == null)
			{
				Logger.LogDebug("UserViewModel.ExecuteGetUserCommand - new user");
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

		private async Task OnSave()
		{
			Logger.LogDebug("UserViewModel.OnSave");
			var isNewUser = await Save();
			if (isNewUser)
			{
				((AppShell)Shell.Current).OpenSettingsPage();
			}
			else
			{
				//await Shell.Current.GoToAsync("..");
				((AppShell)Shell.Current).OpenFridgeContentPage();
			}
		}


		private bool ValidateSave()
		{
			if (String.IsNullOrWhiteSpace(Name))
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

		private void OnReset(object obj)
		{
			FridgeDal.ResetRepository();

			UserId = Guid.Empty;
			Name = string.Empty;
			Email = string.Empty;
		}
	}
}
