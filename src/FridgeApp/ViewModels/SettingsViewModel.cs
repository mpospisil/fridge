using FridgeApp.Services;
using FridgeApp.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
			Title = Resources.Settings;
			Fridges = new ObservableCollection<FridgeViewModel>();
			LoadFridgesCommand = new Command(async () => await ExecuteLoadItemsCommand());

			FridgeDetailsCommand = new Command<FridgeViewModel>(OnShowFridgeDetails);
			AddFridgeCommand = new Command(OnAddFridge);
		}

		private FridgeViewModel selectedFridge;

		public ObservableCollection<FridgeViewModel> Fridges { get; }
		public Command LoadFridgesCommand { get; }
		public Command AddFridgeCommand { get; }

		public Command<FridgeViewModel> FridgeDetailsCommand { get; }

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
				OnShowFridgeDetails(value);
			}
		}

		private async void OnAddFridge(object obj)
		{
			await Shell.Current.GoToAsync($"{nameof(FridgeEditPage)}?{nameof(FridgeViewModel.FridgeId)}={Guid.Empty.ToString()}");
		}

		async void OnShowFridgeDetails(FridgeViewModel item)
		{
			if (item == null)
				return;

			// This will push the ItemDetailPage onto the navigation stack
			await Shell.Current.GoToAsync($"{nameof(FridgeEditPage)}?{nameof(FridgeViewModel.FridgeId)}={item.FridgeId}");
		}
	}
}
