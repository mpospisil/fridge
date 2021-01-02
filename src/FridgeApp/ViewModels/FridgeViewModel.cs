using FridgeApp.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
		private Guid fridgeGuid;
		private DateTime timeStamp;

		public FridgeViewModel() : this(null)
		{

		}

		public FridgeViewModel(IFridgeDAL fridgeDal) : this(fridgeDal, null)
		{
		}

		public FridgeViewModel(IFridgeDAL fridgeDal, Fridge.Model.Fridge fridge) : base(fridgeDal)
		{
			Partitions = new ObservableCollection<PartitionViewModel>();
			if (fridge != null)
			{
				SetPropertiesInVM(fridge);
			}
			
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

		public DateTime TimeStamp
		{
			get => timeStamp;
			set => SetProperty(ref timeStamp, value);
		}

		public ObservableCollection<PartitionViewModel> Partitions { get; }

		/// <summary>
		/// The unique identifier of the fridge
		/// </summary>
		public string FridgeId
		{
			get => fridgeGuid.ToString();
			set
			{
				Guid newId = Guid.Parse(value);

				if (newId != Guid.Empty)
				{
					// existing fridge
					SetProperty(ref fridgeGuid, newId);
					LoadItemId(newId);
				}
				else
				{
					// create temporary fridge
					fridgeGuid = Guid.Empty;
					Name = Resources.NewFridge;
				}
			}
		}
	
		private async void LoadItemId(Guid fridgeId)
		{
			try
			{
				var item = await this.FridgeDal.GetFridgeAsync(fridgeId);
				SetPropertiesInVM(item);
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
			if(fridgeGuid == Guid.Empty)
			{
				// new fridge
				var newFridge = FridgeFromVM();
				newFridge.FridgeId = Guid.NewGuid();
				await FridgeDal.AddFridge(newFridge);
			}
			else
			{
				// existing fridge
				var updatedFridge = FridgeFromVM();
				await FridgeDal.UpdateFridge(updatedFridge);
			}

			// This will pop the current page off the navigation stack
			await Shell.Current.GoToAsync("..");
		}

		private void SetPropertiesInVM(Fridge.Model.Fridge fridge)
		{
			this.fridgeGuid = fridge.FridgeId;
			this.Name = fridge.Name;
			this.TimeStamp = fridge.TimeStamp;

			Partitions.Clear();

			foreach (var partition in fridge.Partitions)
			{
				Partitions.Add(new PartitionViewModel(FridgeDal, partition));
			}
		}

		private Fridge.Model.Fridge FridgeFromVM()
		{
			var fridgeDataFromVM = new Fridge.Model.Fridge();
			fridgeDataFromVM.FridgeId = fridgeGuid;
			fridgeDataFromVM.Name = Name;

			return fridgeDataFromVM;
		}
	}
}
