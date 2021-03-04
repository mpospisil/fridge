using FridgeApp.Services;
using FridgeApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FridgeApp.ViewModels
{
	public interface IItemsViewModel
	{
		void OnAppearing();
		ObservableCollection<IItemViewModel> Items { get; }
		Command LoadItemsCommand { get; }
	}

	public class ItemsViewModel : BaseViewModel, IItemsViewModel
	{
		private IItemViewModel selectedItem;

		public ItemsViewModel(IFridgeDAL fridgeDal) : base(fridgeDal)
		{
			Items = new ObservableCollection<IItemViewModel>();
			LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
			ShowItemDetailsCommand = new Command(OnShowItemDetails, IsItemSelected);
			SelectItemCommand = new Command(OnSelectItem);
		}
		/// <summary>
		/// All items for the user
		/// </summary>
		public ObservableCollection<IItemViewModel> Items { get; private set; }

		public IItemViewModel SelectedItem
		{
			get => selectedItem;
			set
			{
				SetProperty(ref selectedItem, value);
				ShowItemDetailsCommand.ChangeCanExecute();
				//RemoveItemCommand.ChangeCanExecute();
			}
		}

		public Command LoadItemsCommand { get; private set; }
		public Command ShowItemDetailsCommand { get; }
		public Command SelectItemCommand { get; }

		public void OnAppearing()
		{
			IsBusy = true;
		}

		private async void OnShowItemDetails(object obj)
		{
			IItemViewModel selectedItemVM = obj as IItemViewModel;
			await Shell.Current.GoToAsync($"{nameof(ItemPage)}?{nameof(ItemViewModel.ItemFromRepositoryId)}={SelectedItem.ItemId}");
		}

		private bool IsItemSelected(object obj)
		{
			return SelectedItem != null;
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

		/// <summary>
		/// Get all items and store them in the collection Items
		/// </summary>
		/// <returns></returns>
		async Task ExecuteLoadItemsCommand()
		{
			IsBusy = true;

			var fridges = await FridgeDal.GetFridgesAsync(true);

			Dictionary<Guid, PartitionDescriptor> partitionDescriptorDict = new Dictionary<Guid, PartitionDescriptor>();

			int fridgeInx = 0;
			foreach (var fridge in fridges)
			{
				int partitionInx = 0;
				foreach (var partition in fridge.Partitions)
				{
					partitionDescriptorDict.Add(partition.PartitionId, new PartitionDescriptor()
					{
						FridgeInx = fridgeInx,
						PartitionInx = partitionInx,
						Fridge = fridge,
						Partition = partition,
					});

					partitionInx++;
				}
				fridgeInx++;
			}

			try
			{
				Items.Clear();
				var items = await FridgeDal.GetItemsAsync(true);
				foreach (var item in items)
				{
					PartitionDescriptor partitionDescriptor = null;
					if (!partitionDescriptorDict.TryGetValue(item.PartitionId, out partitionDescriptor))
					{
						// partition doesn't exist
						continue;
					}

					var itemVM = new ItemViewModel(FridgeDal, item);

					itemVM.PartitionIndex = partitionDescriptor.PartitionInx;
					itemVM.FridgeIndex = partitionDescriptor.FridgeInx;
					itemVM.FridgeName = partitionDescriptor.Fridge.Name;
					itemVM.PartitionName = partitionDescriptor.Partition.Name;
					Items.Add(itemVM);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
			finally
			{
				IsBusy = false;
			}
		}
	}
}
