using core.Services.Logging;
using System;
using System.Threading.Tasks;

namespace ilvlbot.Services.Logging
{
	/// <summary>
	/// A simple to-the-screen logger.
	/// </summary>
	internal class ConsoleLogger : ILogger
	{
		public void Log(string tag, string logMessage)
		{
			// logging from discord.net calls don't like to hang,
			// so we'll fire and forget a write to the console.
			Task.Run(() =>
			{
				Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] [{tag}] {logMessage}");
			});
		}
	}
}
