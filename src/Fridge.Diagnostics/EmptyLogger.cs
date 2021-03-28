using FridgeApp.Services;
using System;

namespace Fridge.Logger
{
	public class EmptyLogger : IFridgeLogger
	{
		public void LogDebug(string message)
		{
		}

		public void LogError(string message, Exception e)
		{
		}

		public void LogInformation(string message)
		{
		}

		public void LogTrace(string message)
		{
		}
	}
}
