using FridgeApp.Services;
using FridgeApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;

namespace FridgeApp.ViewModels
{
	public interface IMainViewModel
	{
		void OnAppearing();
		ObservableCollection<IItemViewModel> Items { get; }

		Command LoadItemsCommand { get; }
	}

	/// <summary>
	/// The view model for the main screen
	/// </summary>
	public class MainViewModel : BaseViewModel, IMainViewModel
	{
		
		public MainViewModel(IFridgeDAL fridgeDal) : base(fridgeDal)
		{
			Items = new ObservableCollection<IItemViewModel>();
			Fridges = new ObservableCollection<IFridgeViewModel>();
			LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
			LoadFridgesCommand = new Command(async () => await ExecuteLoadFridges());
		}

		/// <summary>
		/// All fridges for the user
		/// </summary>
		public ObservableCollection<IItemViewModel> Items { get; private set; }

		/// <summary>
		/// All items for the user
		/// </summary>
		public ObservableCollection<IFridgeViewModel> Fridges { get; private set; }

		public Command LoadItemsCommand { get; private set; }

		public Command LoadFridgesCommand { get; private set; }

		public void OnAppearing()
		{
			IsBusy = true;
		}

		/// <summary>
		/// Get all items and store them in the collection Items
		/// </summary>
		/// <returns></returns>
		async Task ExecuteLoadItemsCommand()
		{
			IsBusy = true;

			var fridges = await FridgeDal.GetFridgesAsync(true);

			// TODO - move to functionality to DAL
			Dictionary<Guid, Fridge.Model.Fridge> fridgeDict = fridges.ToDictionary(f => f.FridgeId);
			Dictionary<Guid, Tuple<Guid, Fridge.Model.Partition>> partitionDict = new Dictionary<Guid, Tuple<Guid, Fridge.Model.Partition>>();
			foreach(var fridge in fridges)
			{
				foreach(var partition in fridge.Partitions)
				{
					partitionDict.Add(partition.PartitionId, new Tuple<Guid, Fridge.Model.Partition>(fridge.FridgeId, partition));
				}
			}

			try
			{
				Items.Clear();
				var items = await FridgeDal.GetItemsAsync(true);
				foreach (var item in items)
				{
					var itemVM = new ItemViewModel(FridgeDal, item);
					itemVM.FridgeName = fridgeDict[item.FridgeId].Name;
					var partTuple = partitionDict[item.PartitionId];
					itemVM.PartitionName = partTuple.Item2.Name;
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

		/// <summary>
		/// Get all fridges and store them to the collection Fridges 
		/// </summary>
		/// <returns></returns>
		async Task ExecuteLoadFridges()
		{
			IsBusy = true;

			try
			{
				Fridges.Clear();
				var fridges = await FridgeDal.GetFridgesAsync(true);
				foreach (var fridge in fridges)
				{
					var fridgeVM = new FridgeViewModel(FridgeDal, fridge);
					Fridges.Add(fridgeVM);
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
