using FridgeApp.Views;
using Xamarin.Forms;

namespace FridgeApp
{
	public partial class AppShell : Xamarin.Forms.Shell
	{
		public AppShell()
		{
			InitializeComponent();
			Routing.RegisterRoute(nameof(FridgeEditPage), typeof(FridgeEditPage));
			Routing.RegisterRoute(nameof(ItemPage), typeof(ItemPage));
			Routing.RegisterRoute(nameof(UserPage), typeof(UserPage));
		}

		public void OpenUserPage()
		{
			CurrentItem = userPage;
		}

		public void OpenSettingsPage()
		{
			CurrentItem = settingsPage;
		}
	}
}
