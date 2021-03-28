using System;

namespace FridgeApp.Services
{
	public interface IFridgeLogger
	{
		/// <summary>
		/// Logs an error message.
		/// </summary>
		/// <param name="message">Message to log.</param>
		/// <param name="e">Exception to log.</param>
		void LogError(string message, Exception e);

		/// <summary>
		/// Logs info message.
		/// </summary>
		/// <param name="message">Mesage to log.</param>
		void LogInformation(string message);

		/// <summary>
		/// Logs debug message.
		/// </summary>
		/// <param name="message">Mesage to log.</param>
		void LogDebug(string message);
	}
}
