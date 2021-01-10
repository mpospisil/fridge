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
		ObservableCollection<ItemViewModel> Items { get; }

		Command LoadItemsCommand { get; }
	}

	public class MainViewModel : BaseViewModel, IMainViewModel
	{
		
		public MainViewModel(IFridgeDAL fridgeDal) : base(fridgeDal)
		{
			Items = new ObservableCollection<ItemViewModel>();
			LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
		}

		public ObservableCollection<ItemViewModel> Items { get; private set; }

		public Command LoadItemsCommand { get; private set; }

		public void OnAppearing()
		{
			IsBusy = true;
		}

		async Task ExecuteLoadItemsCommand()
		{
			IsBusy = true;

			try
			{
				Items.Clear();
				var items = await FridgeDal.GetItemsAsync(true);
				foreach (var item in items)
				{
					var fridgeVM = new ItemViewModel(FridgeDal, item);
					Items.Add(fridgeVM);
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
