using FridgeApp.Services;
using FridgeApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FridgeApp.ViewModels
{
	public interface ISettingsViewModel
	{
		void OnAppearing();
		ObservableCollection<FridgeViewModel> Fridges { get; }
	}

	public class SettingsViewModel : BaseViewModel, ISettingsViewModel
	{
		public SettingsViewModel(IFridgeDAL fridgeDal) : base(fridgeDal)
		{
			Title = "Settings";
			Fridges = new ObservableCollection<FridgeViewModel>();
			LoadFridgesCommand = new Command(async () => await ExecuteLoadItemsCommand());

			FridgeTapped = new Command<FridgeViewModel>(OnFridgeSelected);

			AddFridgeCommand = new Command(OnAddFridge);
		}

		private FridgeViewModel selectedFridge;

		public ObservableCollection<FridgeViewModel> Fridges { get; }
		public Command LoadFridgesCommand { get; }
		public Command AddFridgeCommand { get; }

		public Command<FridgeViewModel> FridgeTapped { get; }

		private SettingsViewModel()
		{

		}

		async Task ExecuteLoadItemsCommand()
		{
			IsBusy = true;

			try
			{
				Fridges.Clear();
				var items = await FridgeDal.GetFridgesAsync(true);
				foreach (var item in items)
				{
					var fridgeVM = new FridgeViewModel(FridgeDal, item);
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

		public void OnAppearing()
		{
			IsBusy = true;
			SelectedItem = null;
		}

		public FridgeViewModel SelectedItem
		{
			get => selectedFridge;
			set
			{
				SetProperty(ref selectedFridge, value);
				OnFridgeSelected(value);
			}
		}

		private async void OnAddFridge(object obj)
		{
			//await Shell.Current.GoToAsync(nameof(NewItemPage));
		}

		async void OnFridgeSelected(FridgeViewModel item)
		{
			if (item == null)
				return;

			// This will push the ItemDetailPage onto the navigation stack
			await Shell.Current.GoToAsync($"{nameof(FridgePage)}?{nameof(FridgeViewModel.FridgeId)}={item.FridgeId}");
		}
	}
}
