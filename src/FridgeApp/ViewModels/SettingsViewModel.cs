using FridgeApp.Services;
using FridgeApp.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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
		private readonly IFridgeLogger Logger;

		public SettingsViewModel(IFridgeDAL fridgeDal, IFridgeLogger logger) : base(fridgeDal)
		{
			this.Logger = logger;
			Title = Resources.FridgeSettings;
			Fridges = new ObservableCollection<FridgeViewModel>();
			LoadFridgesCommand = new Command(async () => await ExecuteLoadItemsCommand());

			FridgeDetailsCommand = new Command<FridgeViewModel>(OnShowFridgeDetails);
			AddFridgeCommand = new Command(OnAddFridge);
			GoToProductsCommand = new Command(GoToProducts);
		}

		private FridgeViewModel selectedFridge;

		public ObservableCollection<FridgeViewModel> Fridges { get; }
		public Command LoadFridgesCommand { get; }
		public Command AddFridgeCommand { get; }

		public Command GoToProductsCommand { get; }

		public Command<FridgeViewModel> FridgeDetailsCommand { get; }

		private SettingsViewModel()
		{

		}

		async Task ExecuteLoadItemsCommand()
		{
			Logger.LogDebug("SettingsViewModel.ExecuteLoadItemsCommand");
			IsBusy = true;

			try
			{
				Fridges.Clear();
				var items = await FridgeDal.GetFridgesAsync(true);
				foreach (var item in items)
				{
					var fridgeVM = new FridgeViewModel(Logger, FridgeDal, item);
					Fridges.Add(fridgeVM);
				}

				OnPropertyChanged("IsNoFridge");
			}
			catch (Exception ex)
			{
				Logger.LogError("SettingsViewModel.ExecuteLoadItemsCommand", ex);
			}
			finally
			{
				IsBusy = false;
			}
		}

		public bool IsNoFridge
		{
			get
			{
				return !Fridges.Any();
			}
		}

		public void OnAppearing()
		{
			Logger.LogDebug("SettingsViewModel.OnAppearing");
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
			Logger.LogDebug("SettingsViewModel.OnAddFridge");
			await Shell.Current.GoToAsync($"{nameof(FridgeEditPage)}?{nameof(FridgeViewModel.FridgeId)}={Guid.Empty.ToString()}");
		}

		async void OnShowFridgeDetails(FridgeViewModel item)
		{
			Logger.LogDebug("SettingsViewModel.OnShowFridgeDetails");
			if (item == null)
				return;

			// This will push the ItemDetailPage onto the navigation stack
			await Shell.Current.GoToAsync($"{nameof(FridgeEditPage)}?{nameof(FridgeViewModel.FridgeId)}={item.FridgeId}");
		}

		private void GoToProducts()
		{
			Logger.LogDebug("SettingsViewModel.GoToProducts");
			((AppShell)Shell.Current).OpenFridgeContentPage();
		}
	}
}
