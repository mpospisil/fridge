using FridgeApp.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FridgeApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ItemPage : ContentPage
	{
		public ItemPage()
		{
			InitializeComponent();
			BindingContext = DependencyService.Resolve<IItemViewModel>();
		}

		protected override void OnAppearing()
		{
			Shell.SetTabBarIsVisible(this, false);
			base.OnAppearing();
		}
	}
}