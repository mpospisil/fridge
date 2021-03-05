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
		string Query { get; }
		Command LoadItemsCommand { get; }
	}

	public class ItemsViewModel : BaseViewModel, IItemsViewModel
	{
		private IItemViewModel selectedItem;
		private string query;
		private bool isSearching;
		private bool searchAgain;

		public event EventHandler ItemFilterEvent;

		public ItemsViewModel(IFridgeDAL fridgeDal) : base(fridgeDal)
		{
			Items = new ObservableCollection<IItemViewModel>();
			LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
			RemoveItemCommand = new Command(OnRemoveItem, IsItemSelected);
			ShowItemDetailsCommand = new Command(OnShowItemDetails, IsItemSelected);
			SelectItemCommand = new Command(OnSelectItem);

			this.PropertyChanged +=
					(_, __) => RemoveItemCommand.ChangeCanExecute();
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
				RemoveItemCommand.ChangeCanExecute();
			}
		}

		public string Query
		{
			get => query;
			set
			{
				SetProperty(ref query, value);
				OnQueryChanged();
			}
		}

		async void OnQueryChanged()
		{
			if (isSearching)
			{
				searchAgain = true;
				return;
			}

			isSearching = true;

			await SetFilter(Query, Items);

			if (searchAgain)
			{
				await SetFilter(Query, Items);
			}

			searchAgain = false;
			isSearching = false;

			if(ItemFilterEvent != null)
			{
				ItemFilterEvent.Invoke(this, new EventArgs());
			}
		}

		public Command LoadItemsCommand { get; private set; }
		public Command ShowItemDetailsCommand { get; }
		public Command SelectItemCommand { get; }
		public Command RemoveItemCommand { get; }

		public void OnAppearing()
		{
			IsBusy = true;
			SelectedItem = null;
			Query = string.Empty;
		}

		private async static Task SetFilter(string userQuery, IList<IItemViewModel> allItems)
		{
			Debug.WriteLine($"Query = '{userQuery}'");

			var foundItems = await GetSearchTask(userQuery, allItems);

			Debug.Assert(allItems.Count == foundItems.Count);

			for(int i = 0; i < allItems.Count; i++)
			{
				var item = allItems[i];
				item.IsVisible = foundItems[i];
			}
		}

		private static Task<List<bool>> GetSearchTask(string query , IList<IItemViewModel> itemsToSearch)
		{
			return Task.Run<List<bool>>(() =>
			{

				List<bool> res = new List<bool>(itemsToSearch.Count);
				var capitalisedQuery = query.ToLower();

				foreach(var item in itemsToSearch)
				{
					var capItemName = item.Name.ToLower();
					bool searchRes = capItemName.Contains(capitalisedQuery);
					res.Add(searchRes);
				}
				
				return res;
			});
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
		/// Event handler for removing  item from the fridge 
		/// </summary>
		private async void OnRemoveItem(object obj)
		{
			IItemViewModel itemToRemoveVM = SelectedItem;
			// ask user if he really wants to delete the selected item from the fridge
			var answer = await App.Current.MainPage.DisplayAlert(Resources.Verification, String.Format(Resources.Question_Remove_Format, itemToRemoveVM.Name), Resources.Yes, Resources.No);

			if (!answer)
			{
				// leave - the user doesn't want to remove the selected item
				return;
			}

			// remove the selected item
			var fridge = await FridgeDal.GetFridgeAsync(Guid.Parse(itemToRemoveVM.FridgeId));
			Guid itemId = Guid.Parse(itemToRemoveVM.ItemId);
			Guid partitionId = Guid.Parse(itemToRemoveVM.PartitionId);
			await itemToRemoveVM.RemoveItemFromFridge(itemId, fridge.RemovedItemsIdentifier);

			Items.Remove(itemToRemoveVM);
			SelectedItem = null;
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
