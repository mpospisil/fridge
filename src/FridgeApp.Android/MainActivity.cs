
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Fridge.Logger;
using FridgeApp.Services;
using Serilog;

namespace FridgeApp.Droid
{
	[Activity(Label = "FridgeApp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			var logDir = Xamarin.Essentials.FileSystem.AppDataDirectory;
			string logFile = System.IO.Path.Combine(logDir, "fridge.log");

#if DEBUG
			var seriLogger = new LoggerConfiguration()
								.MinimumLevel.Debug()
								.WriteTo.AndroidLog()
								//.WriteTo.File(logFile, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
								.CreateLogger();
#else
			var seriLogger = new LoggerConfiguration()
								.MinimumLevel.Error()
								.WriteTo.AndroidLog()
								//.WriteTo.File(logFile, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}", fileSizeLimitBytes = 100*1024)
								.CreateLogger();
#endif
			var fridgeLogger = new FridgeSerilog(seriLogger);
			App.RegisterInstance<IFridgeLogger>(fridgeLogger);
			FridgeApp.App.FridgeLogger = fridgeLogger;

			App.RegisterTypes();
			App.BuildContainer();

			savedInstanceState = null;

			base.OnCreate(savedInstanceState);

			Xamarin.Essentials.Platform.Init(this, savedInstanceState);
			global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
			LoadApplication(new App());
		}
		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
		{
			Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}
	}
}