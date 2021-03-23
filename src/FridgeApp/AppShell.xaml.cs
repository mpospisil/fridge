using FridgeApp.Services;
using FridgeApp.Views;
using Xamarin.Forms;

namespace FridgeApp
{
	public partial class AppShell : Xamarin.Forms.Shell
	{
		private readonly IFridgeLogger Logger;
		public AppShell(IFridgeLogger logger)
		{
			Logger = logger;
			InitializeComponent();
			Routing.RegisterRoute(nameof(FridgeEditPage), typeof(FridgeEditPage));
			Routing.RegisterRoute(nameof(ItemPage), typeof(ItemPage));
			Routing.RegisterRoute(nameof(UserPage), typeof(UserPage));
		}

		public void OpenUserPage()
		{
			Logger.LogDebug("AppShell.OpenUserPage");
			CurrentItem = userPage;
		}

		public void OpenSettingsPage()
		{
			Logger.LogDebug("AppShell.OpenSettingsPage");
			CurrentItem = settingsPage;
		}

		public void OpenFridgeContentPage()
		{
			Logger.LogDebug("AppShell.OpenFridgeContentPage");
			CurrentItem = fridgeContentPage;
		}
	}
}
