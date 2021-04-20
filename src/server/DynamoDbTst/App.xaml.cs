using Fridge.Logger;
using FridgeApp.Services;
using Serilog;
using Serilog.Events;
using System.Windows;

namespace DynamoDbTst
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public static IFridgeLogger Logger;
		private void Application_Startup(object sender, StartupEventArgs e)
		{
			var serilog = new LoggerConfiguration()
				.MinimumLevel.Debug()
				.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
				.MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
				.MinimumLevel.Override("System", LogEventLevel.Warning)
				.Enrich.FromLogContext()
				.WriteTo.Debug()
				.CreateLogger();

			Logger = new FridgeSerilog(serilog);
		}
	}
}
