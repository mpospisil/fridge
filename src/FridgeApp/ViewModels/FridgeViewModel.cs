using FridgeApp.Services;
using FridgeApp.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;

namespace FridgeApp.ViewModels
{
	/// <summary>
	/// The view model model for a fridge
	/// </summary>
	public interface IFridgeViewModel
	{
		string Name { get; set; }
		string FridgeId { get; set; }
	}

	[QueryProperty(nameof(FridgeId), nameof(FridgeId))]
	public class FridgeViewModel : BaseViewModel, IFridgeViewModel
	{
		#region Private fields
		private string name;
		private Guid fridgeGuid;
		private Guid removedItemsIdentifier;
		private DateTime timeStamp;
		private IPartitionViewModel selectedPartition;
		private IItemViewModel selectedItem;
		#endregion

		#region Consructors
		public FridgeViewModel() : this(null)
		{
		}

		public FridgeViewModel(IFridgeDAL fridgeDal) : this(fridgeDal, null)
		{
		}

		public FridgeViewModel(IFridgeDAL fridgeDal, Fridge.Model.Fridge fridge) : base(fridgeDal)
		{
			Partitions = new ObservableCollection<IPartitionViewModel>();
			if (fridge != null)
			{
				SetPropertiesInVM(fridge);
			}

			SaveCommand = new Command(OnSave, ValidateSave);
			CancelCommand = new Command(OnCancel);
			AddPartitionCommand = new Command(AddPartition);
			DeletePartitionCommand = new Command(DeletePartition);
			DeleteFridgeCommand = new Command(OnDeleteFridge);
			AddItemCommand = new Command(OnAddItem, CanAddItem);
			ShowItemDetailsCommand = new Command(OnShowItemDetails, IsItemSelected);
			RemoveItemCommand = new Command(OnRemoveItem, IsItemSelected);
			SelectItemCommand = new Command(OnSelectItem);

			this.PropertyChanged +=
					(_, __) => SaveCommand.ChangeCanExecute();
		}
		#endregion

		#region Properties

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

		public ObservableCollection<IPartitionViewModel> Partitions { get; }

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
		/// Selected partition in the fridge
		/// </summary>
		public IPartitionViewModel SelectedPartition
		{
			get => selectedPartition;
			set
			{
				SetProperty(ref selectedPartition, value);
				value?.LoadItemsCommand?.Execute(null);
				AddItemCommand.ChangeCanExecute();
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
		public Command AddPartitionCommand { get; }
		public Command DeletePartitionCommand { get; }
		public Command DeleteFridgeCommand { get; }
		public Command AddItemCommand { get; }
		public Command ShowItemDetailsCommand { get; }
		public Command RemoveItemCommand { get; }
		public Command SelectItemCommand { get; }

		public void AddPartition()
		{
			try
			{
				var newPartition = new Fridge.Model.Partition();
				newPartition.Name = Resources.NewPartition;
				Partitions.Add(new PartitionViewModel(FridgeDal, newPartition));
			}
			catch (Exception e)
			{
				Debug.WriteLine($"Failed to add partition {e.Message}");
			}
		}

		public async Task DeleteFridgeAsync()
		{
			await FridgeDal.DeleteFridgeAsync(fridgeGuid);
		}

		private async void OnDeleteFridge()
		{
			await DeleteFridgeAsync();

			// This will pop the current page off the navigation stack
			await Shell.Current.GoToAsync("..");
		}

		public void DeletePartition(object obj)
		{
			try
			{
				IPartitionViewModel partitionToDelete = obj as IPartitionViewModel;
				var partitionIndexToDelete = Partitions.IndexOf(partitionToDelete);
				Partitions.RemoveAt(partitionIndexToDelete);
			}
			catch (Exception e)
			{
				Debug.WriteLine($"Failed to delete partition {e.Message}");
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
				await FridgeDal.AddFridge(newFridge);
			}
			else
			{
				// existing fridge
				var updatedFridge = FridgeFromVM();
				await FridgeDal.UpdateFridge(updatedFridge);
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
			return SelectedPartition != null;
		}

		private async void OnAddItem(object obj)
		{
			await Shell.Current.GoToAsync($"{nameof(NewItemPage)}?{nameof(ItemViewModel.ItemId)}={Guid.Empty.ToString()}&{nameof(ItemViewModel.FridgeId)}={FridgeId.ToString()}&{nameof(ItemViewModel.PartitionId)}={SelectedPartition.PartitionId.ToString()}");
		}

		private void OnShowItemDetails(object obj)
		{

		}

		/// <summary>
		/// Event handler for removing  item from the fridge 
		/// </summary>
		/// <param name="obj"></param>
		private async void OnRemoveItem(object obj)
		{
			IItemViewModel itemToRemoveVM = obj as IItemViewModel;
			// ask user if he really wants to delete the selected item from the fridge
			var answer = await App.Current.MainPage.DisplayAlert("Question?", String.Format(Resources.Question_Remove_Format, itemToRemoveVM.Name), Resources.Yes, Resources.No);

			if (!answer)
			{
				// leave - the user doesn't want to remove the selected item
				return;
			}

			// remove the selected item
			Guid itemId = Guid.Parse(itemToRemoveVM.ItemId);
			Guid partitionId = Guid.Parse(itemToRemoveVM.PartitionId);
			await RemoveItemFromFridge(itemId);

			Partitions.First(p => p.PartitionId == partitionId).Items.Remove(itemToRemoveVM);
		}

		public async Task RemoveItemFromFridge(Guid itemId)
		{
			var itemToRemove = await FridgeDal.GetItemAsync(itemId);
			
			// set FrigeId and PartitionId to value of RemovedItemsIdentifier for this fridge
			itemToRemove.FridgeId = RemovedItemsIdentifier;
			itemToRemove.PartitionId = RemovedItemsIdentifier;
			itemToRemove.TimeStamp = DateTime.UtcNow;
			itemToRemove.History.Add(new Fridge.Model.ItemChange() { TimeOfChange = itemToRemove.TimeStamp, TypeOfChange = Fridge.Model.ChangeTypes.Removed });
			await FridgeDal.UpdateItemAsync(itemToRemove);
		}

		private void OnSelectItem(object obj)
		{
			SelectedItem = obj as IItemViewModel;
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

			Partitions.Clear();

			foreach (var partition in fridge.Partitions)
			{
				Partitions.Add(new PartitionViewModel(FridgeDal, partition));
			}
		}

		public Fridge.Model.Fridge FridgeFromVM()
		{
			var fridgeDataFromVM = new Fridge.Model.Fridge();
			fridgeDataFromVM.FridgeId = fridgeGuid;
			fridgeDataFromVM.RemovedItemsIdentifier = RemovedItemsIdentifier;
			fridgeDataFromVM.Name = Name;

			fridgeDataFromVM.TimeStamp = TimeStamp;

			foreach (var partitionVM in Partitions)
			{
				fridgeDataFromVM.Partitions.Add(partitionVM.PartitionFromVM());
			}

			return fridgeDataFromVM;
		}

		private bool ValidateSave()
		{
			return !String.IsNullOrWhiteSpace(Name);
		}
	}
}
