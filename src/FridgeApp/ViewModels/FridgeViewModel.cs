using FridgeApp.Services;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FridgeApp.ViewModels
{
	public interface IFridgeViewModel
	{
		string Name { get; set; }
		string FridgeId { get; set; }
	}

	[QueryProperty(nameof(FridgeId), nameof(FridgeId))]
	public class FridgeViewModel : BaseViewModel, IFridgeViewModel
	{
		private string name;
		private string description;
		private Guid fridge;

		private Fridge.Model.Fridge Fridge { get; set; }

		public FridgeViewModel() : this(null)
		{

		}

		public FridgeViewModel(IFridgeDAL fridgeDal) : this(fridgeDal, null)
		{
		}

		public FridgeViewModel(IFridgeDAL fridgeDal, Fridge.Model.Fridge fridge) : base(fridgeDal)
		{
			if (fridge != null)
			{
				this.Fridge = fridge;
				this.name = fridge.Name;
				this.fridge = fridge.FridgeId;
			}

			Description = string.Empty;
			SaveCommand = new Command(OnSave, ValidateSave);
			CancelCommand = new Command(OnCancel);
			this.PropertyChanged +=
					(_, __) => SaveCommand.ChangeCanExecute();
		}

		private bool ValidateSave()
		{
			return !String.IsNullOrWhiteSpace(Name);
		}


		public string Name
		{
			get => name;
			set => SetProperty(ref name, value);
		}

		public string Description
		{
			get => description;
			set => SetProperty(ref description, value);
		}

		/// <summary>
		/// The unique identifier of the fridge
		/// </summary>
		public string FridgeId
		{
			get => fridge.ToString();
			set
			{
				Guid newId = Guid.Parse(value);
				SetProperty(ref fridge, newId);
				LoadItemId(newId);
			}
		}

		public async void LoadItemId(Guid fridgeId)
		{
			try
			{
				var item = await this.FridgeDal.GetFridgeAsync(fridgeId);
				this.Fridge = item;
				this.Name = item.Name;
				this.FridgeId = item.FridgeId.ToString();
			}
			catch (Exception)
			{
				Debug.WriteLine("Failed to Load Item");
			}
		}

		public Command SaveCommand { get; }
		public Command CancelCommand { get; }

		private async void OnCancel()
		{
			// This will pop the current page off the navigation stack
			await Shell.Current.GoToAsync("..");
		}

		private async void OnSave()
		{
			//Item newItem = new Item()
			//{
			//	Id = Guid.NewGuid().ToString(),
			//	Text = Text,
			//	Description = Description
			//};

			//await DataStore.AddItemAsync(newItem);

			// This will pop the current page off the navigation stack
			await Shell.Current.GoToAsync("..");
		}
	}
}
