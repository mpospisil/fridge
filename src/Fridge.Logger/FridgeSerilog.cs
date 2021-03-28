using FridgeApp.Services;
using System;

namespace Fridge.Logger
{
	public class FridgeSerilog : IFridgeLogger
	{
		private readonly Serilog.Core.Logger Logger;

		public FridgeSerilog(Serilog.Core.Logger seriLogger)
		{
			Logger = seriLogger;
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
