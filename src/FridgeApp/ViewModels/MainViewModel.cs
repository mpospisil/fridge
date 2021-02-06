using FridgeApp.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

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

			try
			{
				Items.Clear();
				var items = await FridgeDal.GetItemsAsync(true);
				foreach (var item in items)
				{
					var itemVM = new ItemViewModel(FridgeDal, item);
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
