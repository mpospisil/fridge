using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;

namespace Fridge.Auth
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Log.Logger = new LoggerConfiguration()
					.MinimumLevel.Debug()
					.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
					.MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
					.MinimumLevel.Override("System", LogEventLevel.Warning)
					.MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
					.Enrich.FromLogContext()

					.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
					.CreateLogger();

			try
			{
				Log.Information("Starting host...");
				CreateHostBuilder(args).Build().Run();

			}
			catch (Exception ex)
			{
				Log.Fatal(ex, "Host terminated unexpectedly.");

			}
			finally
			{
				Log.CloseAndFlush();
			}
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
				Host.CreateDefaultBuilder(args)
						.ConfigureWebHostDefaults(webBuilder =>
						{
							webBuilder.UseStartup<Startup>();
						});
	}
}
