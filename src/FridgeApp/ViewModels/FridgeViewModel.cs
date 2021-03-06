﻿using FridgeApp.Services;
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
		ObservableCollection<IPartitionViewModel> Partitions { get; }
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
			DeleteFridgeCommand = new Command(OnDeleteFridge, CanDeleteFridge);
			AddItemCommand = new Command(OnAddItem, CanAddItem);
			RemoveItemCommand = new Command(OnRemoveItem, IsItemSelected);
			ShowItemDetailsCommand = new Command(OnShowItemDetails, IsItemSelected);
			SelectItemCommand = new Command(OnSelectItem);

			this.PropertyChanged +=
					(_, __) => SaveCommand.ChangeCanExecute();

			this.PropertyChanged +=
					(_, __) => DeletePartitionCommand.ChangeCanExecute();

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
		public Command RemoveItemCommand { get; }
		public Command ShowItemDetailsCommand { get; }
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

		private bool CanDeleteFridge()
		{
			return true;
		}

		private async void OnDeleteFridge()
		{
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
			await Shell.Current.GoToAsync($"{nameof(ItemPage)}?{nameof(ItemViewModel.ItemId)}={Guid.Empty.ToString()}&{nameof(ItemViewModel.FridgeId)}={FridgeId.ToString()}&{nameof(ItemViewModel.PartitionId)}={SelectedPartition.PartitionId.ToString()}");
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
			Guid partitionId = Guid.Parse(itemToRemoveVM.PartitionId);
			await itemToRemoveVM.RemoveItemFromFridge(itemId, RemovedItemsIdentifier);

			Partitions.First(p => p.PartitionId == partitionId).Items.Remove(itemToRemoveVM);
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

			Partitions.Clear();

			foreach (var partition in fridge.Partitions)
			{
				Partitions.Add(new PartitionViewModel(FridgeDal, partition));
			}

			if (SelectedPartition == null)
			{
				SelectedPartition = Partitions?.FirstOrDefault();
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
