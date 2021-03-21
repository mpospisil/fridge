using FridgeApp.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FridgeApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UserPage : ContentPage
	{
		private IUserViewModel viewModel;
		public UserPage()
		{
			InitializeComponent();
			BindingContext = viewModel = DependencyService.Resolve<IUserViewModel>();
		}

		protected override void OnAppearing()
		{
			Shell.SetNavBarIsVisible(this, false);
			base.OnAppearing();
			viewModel.OnAppearing();
		}
	}
}