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
		ObservableCollection<IFridgeViewModel> Fridges { get; }

		Command LoadFridgesCommand { get; }
	}

	/// <summary>
	/// The view model for the main screen
	/// </summary>
	public class MainViewModel : BaseViewModel, IMainViewModel
	{

		public MainViewModel(IFridgeDAL fridgeDal) : base(fridgeDal)
		{
			Fridges = new ObservableCollection<IFridgeViewModel>();
			LoadFridgesCommand = new Command(async () => await ExecuteLoadFridges());
		}

		/// <summary>
		/// All fridges for the user
		/// </summary>
		public ObservableCollection<IFridgeViewModel> Fridges { get; private set; }

		public Command LoadFridgesCommand { get; private set; }

		public void OnAppearing()
		{
			IsBusy = true;
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
