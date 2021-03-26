using FridgeApp.Services;
using FridgeApp.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FridgeApp.ViewModels
{
	/// <summary>
	/// The view model model for a fridge
	/// </summary>
	public interface IFridgeViewModel
	{
		string Name { get; set; }
		string FridgeId { get; set; }
		ObservableCollection<ISectorViewModel> Sectors { get; }
	}

	[QueryProperty(nameof(FridgeId), nameof(FridgeId))]
	public class FridgeViewModel : BaseViewModel, IFridgeViewModel
	{
		#region Private fields
		private string name;
		private Guid fridgeGuid;
		private Guid ownerId;
		private Guid removedItemsIdentifier;
		private DateTime timeStamp;
		private ISectorViewModel selectedSector;
		private IItemViewModel selectedItem;
		private readonly IFridgeLogger Logger;
		#endregion

		#region Consructors
		public FridgeViewModel(IFridgeLogger logger) : this(logger, null)
		{
		}

		public FridgeViewModel(IFridgeLogger logger, IFridgeDAL fridgeDal) : this(logger, fridgeDal, null)
		{
		}

		public FridgeViewModel(IFridgeLogger logger, IFridgeDAL fridgeDal, Fridge.Model.Fridge fridge) : base(fridgeDal)
		{
			Logger = logger;

			Sectors = new ObservableCollection<ISectorViewModel>();
			if (fridge != null)
			{
				SetPropertiesInVM(fridge);
			}

			SaveCommand = new Command(OnSave, ValidateSave);
			CancelCommand = new Command(OnCancel);
			AddSectorCommand = new Command(AddSector);
			DeleteSectorCommand = new Command(DeleteSector);
			DeleteFridgeCommand = new Command(OnDeleteFridge, CanDeleteFridge);
			AddItemCommand = new Command(OnAddItem, CanAddItem);
			RemoveItemCommand = new Command(OnRemoveItem, IsItemSelected);
			ShowItemDetailsCommand = new Command(OnShowItemDetails, IsItemSelected);
			SelectItemCommand = new Command(OnSelectItem);

			this.PropertyChanged +=
					(_, __) => SaveCommand.ChangeCanExecute();

			this.PropertyChanged +=
					(_, __) => DeleteSectorCommand.ChangeCanExecute();

			this.PropertyChanged +=
					(_, __) => DeleteFridgeCommand.ChangeCanExecute();

			this.PropertyChanged +=
					(_, __) => AddItemCommand.ChangeCanExecute();
		}
		#endregion

		#region Properties

		public string Name
		{
			get => name;
			set => SetProperty(ref name, value);
		}

		public Guid OwnerId
		{
			get => ownerId;
			set => SetProperty(ref ownerId, value);
		}

		public DateTime TimeStamp
		{
			get => timeStamp;
			set => SetProperty(ref timeStamp, value);
		}

		public ObservableCollection<ISectorViewModel> Sectors { get; }

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
					SetDefaultFridge();
				}
			}
		}

		private void SetDefaultFridge()
		{
			Logger.LogDebug("FridgeViewModel.SetDefaultFridge");

			// create temporary fridge
			fridgeGuid = Guid.Empty;
			Name = Resources.NewFridge;

			for (int i = 1; i < 4; i++)
			{
				AddSector($"{Resources.Sector} {i}");
			}
		}

		/// <summary>
		/// The unique identifier of the items which were removed from this fridge
		/// </summary>
		public Guid RemovedItemsIdentifier
		{
			get => removedItemsIdentifier;
			set
			{
				SetProperty(ref removedItemsIdentifier, value);
			}
		}

		/// <summary>
		/// Selected sector in the fridge
		/// </summary>
		public ISectorViewModel SelectedSector
		{
			get => selectedSector;
			set
			{
				SetProperty(ref selectedSector, value);
				value?.LoadItemsCommand?.Execute(null);
				AddItemCommand?.ChangeCanExecute();
			}
		}

		public IItemViewModel SelectedItem
		{
			get => selectedItem;
			set
			{
				SetProperty(ref selectedItem, value);
				ShowItemDetailsCommand.ChangeCanExecute();
				RemoveItemCommand.ChangeCanExecute();
			}
		}

		#endregion
		private async void LoadItemId(Guid fridgeId)
		{
			Logger.LogDebug($"FridgeViewModel.LoadItemId '{fridgeId.ToString()}'");
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
		public Command AddSectorCommand { get; }
		public Command DeleteSectorCommand { get; }
		public Command DeleteFridgeCommand { get; }
		public Command AddItemCommand { get; }
		public Command RemoveItemCommand { get; }
		public Command ShowItemDetailsCommand { get; }
		public Command SelectItemCommand { get; }

		public void AddSector(object param)
		{
			try
			{
				var newSector = new Fridge.Model.Sector();
				if (param == null)
				{
					newSector.Name = Resources.NewSector;
				}
				else
				{
					newSector.Name = param.ToString();
				}

				Logger.LogDebug($"FridgeViewModel.AddSector '{newSector.Name}'"); 

				Sectors.Add(new SectorViewModel(Logger, FridgeDal, newSector));
				SaveCommand.ChangeCanExecute();
			}
			catch (Exception e)
			{
				Logger.LogError($"FridgeViewModel.AddSector", e);
			}
		}

		public async Task DeleteFridgeAsync()
		{
			Logger.LogDebug($"FridgeViewModel.DeleteFridgeAsync");
			await FridgeDal.DeleteFridgeAsync(fridgeGuid);
		}

		private bool CanDeleteFridge()
		{
			return true;
		}

		private async void OnDeleteFridge()
		{
			Logger.LogDebug($"FridgeViewModel.OnDeleteFridge");
			// ask user if he really wants to delete the selected item from the fridge
			var answer = await App.Current.MainPage.DisplayAlert(Resources.Verification, String.Format(Resources.Question_Remove_Format, Name), Resources.Yes, Resources.No);

			if (!answer)
			{
				// leave - the user doesn't want to remove the selected item
				return;
			}

			await DeleteFridgeAsync();

			// This will pop the current page off the navigation stack
			await Shell.Current.GoToAsync("..");
		}

		public void DeleteSector(object obj)
		{
			try
			{
				ISectorViewModel sectorToDelete = obj as ISectorViewModel;
				Logger.LogDebug($"FridgeViewModel.DeleteSector '{sectorToDelete.Name}'");
				var sectorIndexToDelete = Sectors.IndexOf(sectorToDelete);
				Sectors.RemoveAt(sectorIndexToDelete);
				SaveCommand.ChangeCanExecute();
			}
			catch (Exception e)
			{
				Logger.LogError("FridgeViewModel.DeleteSector", e);
			}
		}

		private async void OnCancel()
		{
			// This will pop the current page off the navigation stack
			await Shell.Current.GoToAsync("..");
		}

		public async Task SaveData()
		{
			if (fridgeGuid == Guid.Empty)
			{
				// new fridge
				var newFridge = FridgeFromVM();
				newFridge.FridgeId = Guid.NewGuid();
				var user = await FridgeDal.GetUserAsync();
				newFridge.OwnerId = user.UserId;
				await FridgeDal.AddFridgeAsync(newFridge);
			}
			else
			{
				// existing fridge
				var updatedFridge = FridgeFromVM();
				await FridgeDal.UpdateFridgeAsync(updatedFridge);
			}
		}

		private async void OnSave()
		{
			await SaveData();

			// This will pop the current page off the navigation stack
			await Shell.Current.GoToAsync("..");
		}

		private bool CanAddItem(object obj)
		{
			return SelectedSector != null;
		}

		private async void OnAddItem(object obj)
		{
			await Shell.Current.GoToAsync($"{nameof(ItemPage)}?{nameof(ItemViewModel.ItemId)}={Guid.Empty.ToString()}&{nameof(ItemViewModel.FridgeId)}={FridgeId.ToString()}&{nameof(ItemViewModel.SectorId)}={SelectedSector.SectorId.ToString()}");
		}

		private async void OnShowItemDetails(object obj)
		{
			IItemViewModel selectedItemVM = obj as IItemViewModel;
			await Shell.Current.GoToAsync($"{nameof(ItemPage)}?{nameof(ItemViewModel.ItemFromRepositoryId)}={SelectedItem.ItemId}");
		}

		/// <summary>
		/// Event handler for removing  item from the fridge 
		/// </summary>
		/// <param name="obj"></param>
		private async void OnRemoveItem(object obj)
		{
			IItemViewModel itemToRemoveVM = obj as IItemViewModel;
			// ask user if he really wants to delete the selected item from the fridge
			var answer = await App.Current.MainPage.DisplayAlert(Resources.Verification, String.Format(Resources.Question_Remove_Format, itemToRemoveVM.Name), Resources.Yes, Resources.No);

			if (!answer)
			{
				// leave - the user doesn't want to remove the selected item
				return;
			}

			// remove the selected item
			Guid itemId = Guid.Parse(itemToRemoveVM.ItemId);
			Guid sectorId = Guid.Parse(itemToRemoveVM.SectorId);
			await itemToRemoveVM.RemoveItemFromFridge(itemId, RemovedItemsIdentifier);

			Sectors.First(p => p.SectorId == sectorId).Items.Remove(itemToRemoveVM);
		}

		private void OnSelectItem(object obj)
		{
			IItemViewModel newItemVM = obj as IItemViewModel;

			if (SelectedItem == newItemVM)
			{
				// same item - deselect it
				SelectedItem.IsSelected = false;
				SelectedItem = null;
			}
			else
			{
				// different item
				if (SelectedItem != null)
				{
					SelectedItem.IsSelected = false;
					newItemVM.IsSelected = true;
					SelectedItem = newItemVM;
				}
				else
				{
					newItemVM.IsSelected = true;
					SelectedItem = newItemVM;
				}
			}
		}

		private bool IsItemSelected(object obj)
		{
			return SelectedItem != null;
		}

		private void SetPropertiesInVM(Fridge.Model.Fridge fridge)
		{
			this.fridgeGuid = fridge.FridgeId;
			this.RemovedItemsIdentifier = fridge.RemovedItemsIdentifier;
			this.Name = fridge.Name;
			this.TimeStamp = fridge.TimeStamp;
			this.OwnerId = fridge.OwnerId;

			Sectors.Clear();

			foreach (var sector in fridge.Sectors)
			{
				Sectors.Add(new SectorViewModel(Logger, FridgeDal, sector));
			}

			if (SelectedSector == null)
			{
				SelectedSector = Sectors?.FirstOrDefault();
			}
		}

		public Fridge.Model.Fridge FridgeFromVM()
		{
			var fridgeDataFromVM = new Fridge.Model.Fridge();
			fridgeDataFromVM.FridgeId = fridgeGuid;
			fridgeDataFromVM.RemovedItemsIdentifier = RemovedItemsIdentifier;
			fridgeDataFromVM.Name = Name;
			fridgeDataFromVM.OwnerId = OwnerId;

			fridgeDataFromVM.TimeStamp = TimeStamp;

			foreach (var sectorVM in Sectors)
			{
				fridgeDataFromVM.Sectors.Add(sectorVM.SectorFromVM());
			}

			return fridgeDataFromVM;
		}

		private bool ValidateSave()
		{
			if (String.IsNullOrWhiteSpace(Name))
			{
				return false;
			}

			if (!Sectors.Any())
			{
				return false;
			}

			return true;
		}
	}
}
