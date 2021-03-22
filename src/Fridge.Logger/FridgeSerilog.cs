using FridgeApp.Services;
using Serilog;
using System;

namespace Fridge.Logger
{
	public class FridgeSerilog : IFridgeLogger
	{
		private readonly Serilog.Core.Logger Logger;

		public FridgeSerilog(string logFile)
		{
#if DEBUG
			Logger = new LoggerConfiguration()
					.MinimumLevel.Debug()
					.WriteTo.Console()
					.WriteTo.File(logFile,
				outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
					.CreateLogger();
#else
			Logger = new LoggerConfiguration()
				.MinimumLevel.Error()
				.WriteTo.File(logFile,
				outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
					.CreateLogger();
#endif
		}

		public void LogDebug(string message)
		{
			Logger.Debug(message);
		}

		public void LogInformation(string message)
		{
			Logger.Information(message);
		}
		public void LogError(string message, Exception e)
		{
			Logger.Error(e, message);
		}
	}
}
