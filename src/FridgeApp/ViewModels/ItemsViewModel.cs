﻿using Fridge.Model;
using FridgeApp.Comparers;
using FridgeApp.Services;
using FridgeApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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
		private readonly IFridgeLogger Logger;
		private IItemViewModel selectedItem;
		private string query;
		private bool isSearching;
		private bool searchAgain;
		private ItemsOrder sortMethod;
		private ObservableCollection<IItemViewModel> allItems;

		public event EventHandler ItemFilterEvent;

		public ItemsViewModel(IFridgeLogger logger, IFridgeDAL fridgeDal) : base(fridgeDal)
		{
			Logger = logger;
			sortMethod = ItemsOrder.ByFridge;
			allItems = new ObservableCollection<IItemViewModel>();
			LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
			RemoveItemCommand = new Command(OnRemoveItem, IsItemSelected);
			ShowItemDetailsCommand = new Command(OnShowItemDetails, IsItemSelected);
			SelectItemCommand = new Command(OnSelectItem);
			SortItemsCommand = new Command(OnSortItems);

			this.PropertyChanged +=
					(_, __) => RemoveItemCommand.ChangeCanExecute();
		}
		/// <summary>
		/// All items for the user
		/// </summary>
		public ObservableCollection<IItemViewModel> Items
		{
			get => allItems;
			set
			{
				Logger.LogDebug($"ItemsViewModel.Items setting '{value?.Count}'");
				SetProperty(ref allItems, value);
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

		public string Query
		{
			get => query;
			set
			{
				SetProperty(ref query, value);
				OnQueryChanged();
			}
		}

		public ItemsOrder SortMethod
		{
			get => sortMethod;
			set
			{
				SetProperty(ref sortMethod, value);
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

			if (ItemFilterEvent != null)
			{
				ItemFilterEvent.Invoke(this, new EventArgs());
			}
		}

		public Command LoadItemsCommand { get; private set; }
		public Command ShowItemDetailsCommand { get; }
		public Command SelectItemCommand { get; }
		public Command RemoveItemCommand { get; }

		public Command SortItemsCommand { get; }

		public void OnAppearing()
		{
			IsBusy = true;
			SelectedItem = null;
		}

		private async static Task SetFilter(string userQuery, IList<IItemViewModel> allItems)
		{
			Debug.WriteLine($"Query = '{userQuery}'");

			var foundItems = await GetSearchTask(userQuery, allItems);

			Debug.Assert(allItems.Count == foundItems.Count);

			for (int i = 0; i < allItems.Count; i++)
			{
				var item = allItems[i];
				item.IsVisible = foundItems[i];
			}
		}

		private static Task<List<bool>> GetSearchTask(string query, IList<IItemViewModel> itemsToSearch)
		{
			return Task.Run<List<bool>>(() =>
			{

				List<bool> res = new List<bool>(itemsToSearch.Count);
				var capitalisedQuery = query.ToLower();

				foreach (var item in itemsToSearch)
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

		private async void OnSortItems(object obj)
		{
			ItemsOrder itemsOrder = (ItemsOrder)obj;

			if (SortMethod == itemsOrder)
			{
				// nothing is changed - leave
				return;
			}

			await SetSortedItems(itemsOrder, Items);
		}

		public async Task SetSortedItems(ItemsOrder itemsOrder, IEnumerable<IItemViewModel> itemsToSort)
		{
			Logger.LogDebug($"ItemsViewModel.SetSortedItems '{itemsOrder}'");
			var sortedItems = await SortItemsAsync(itemsOrder, itemsToSort);
			Items = new ObservableCollection<IItemViewModel>(sortedItems);
			SortMethod = itemsOrder;
		}

		private async static Task<List<IItemViewModel>> SortItemsAsync(ItemsOrder itemsOrder, IEnumerable<IItemViewModel> itemsToSort)
		{
			return await Task.Run(() =>
			{
				List<IItemViewModel> sortedItems = new List<IItemViewModel>(itemsToSort);

				var comparer = GetComparer(itemsOrder);
				if (comparer == null)
				{
					return itemsToSort.ToList();
				}

				sortedItems.Sort(comparer);

				return sortedItems;
			});
		}

		private static IComparer<IItemViewModel> GetComparer(ItemsOrder itemsOrder)
		{
			switch (itemsOrder)
			{
				case ItemsOrder.ByDate:
					return new ItemDateComparer();

				case ItemsOrder.ByName:
					return new ItemNameComparer();

				case ItemsOrder.ByFridge:
					return new ItemInFridgeComparer();
			}

			return null;
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
			Guid sectorId = Guid.Parse(itemToRemoveVM.SectorId);
			await itemToRemoveVM.RemoveItemFromFridge(itemId);

			Items.Remove(itemToRemoveVM);
			SelectedItem = null;
		}

		/// <summary>
		/// Get all items and store them in the collection Items
		/// </summary>
		/// <returns></returns>
		public async Task ExecuteLoadItemsCommand()
		{
			IsBusy = true;

			var fridges = await FridgeDal.GetFridgesAsync(true);

			Dictionary<Guid, SectorDescriptor> sectorDescriptorDict = new Dictionary<Guid, SectorDescriptor>();

			int fridgeInx = 0;
			foreach (var fridge in fridges)
			{
				int sectorInx = 0;
				foreach (var sector in fridge.Sectors)
				{
					sectorDescriptorDict.Add(sector.SectorId, new SectorDescriptor()
					{
						FridgeInx = fridgeInx,
						SectorInx = sectorInx,
						Fridge = fridge,
						Sector = sector,
					});

					sectorInx++;
				}
				fridgeInx++;
			}

			try
			{
				ObservableCollection<IItemViewModel> newItems = new ObservableCollection<IItemViewModel>();

				var items = await FridgeDal.GetItemsAsync(true);

				foreach (var item in items)
				{
					SectorDescriptor sectorDescriptor = null;
					if (!sectorDescriptorDict.TryGetValue(item.SectorId, out sectorDescriptor))
					{
						// sector doesn't exist
						continue;
					}

					var itemVM = new ItemViewModel(Logger, FridgeDal, item);

					itemVM.SectorIndex = sectorDescriptor.SectorInx;
					itemVM.FridgeIndex = sectorDescriptor.FridgeInx;
					itemVM.FridgeName = sectorDescriptor.Fridge.Name;
					itemVM.SectorName = sectorDescriptor.Sector.Name;
					newItems.Add(itemVM);
				}

				var sortedItems = await SortItemsAsync(sortMethod, newItems);
				Items = new ObservableCollection<IItemViewModel>(sortedItems);

				Query = string.Empty;
			}
			catch (Exception ex)
			{
				Logger.LogError("ItemsViewModel.ExecuteLoadItemsCommand", ex);
			}
			finally
			{
				IsBusy = false;
			}
		}
	}
}
