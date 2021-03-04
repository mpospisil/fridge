using FridgeApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FridgeApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ItemsPage : ContentPage
	{
		IItemsViewModel viewModel;

		public ItemsPage()
		{
			InitializeComponent();
			BindingContext = viewModel = DependencyService.Resolve<IItemsViewModel>();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			Shell.SetTabBarIsVisible(this, true);
			viewModel.OnAppearing();
		}
	}
}