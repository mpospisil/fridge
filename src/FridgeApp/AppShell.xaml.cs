using FridgeApp.Views;
using Xamarin.Forms;

namespace FridgeApp
{
	public partial class AppShell : Xamarin.Forms.Shell
	{
		public AppShell()
		{
			InitializeComponent();
			Routing.RegisterRoute(nameof(FridgePage), typeof(FridgePage));
			Routing.RegisterRoute(nameof(ItemPage), typeof(ItemPage));
		}

		//private async void OnMenuItemClicked(object sender, EventArgs e)
		//{
		//	await Shell.Current.GoToAsync("//LoginPage");
		//}
	}
}
