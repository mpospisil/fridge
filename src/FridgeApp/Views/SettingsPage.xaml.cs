using FridgeApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FridgeApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SettingsPage : ContentPage
	{
		ISettingsViewModel viewModel;

		public SettingsPage()
		{
			InitializeComponent();
			BindingContext = viewModel = DependencyService.Resolve<ISettingsViewModel>();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			viewModel.OnAppearing();
		}
	}
}