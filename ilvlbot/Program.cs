using bnet;
using core.Services.Logging;
using ilvlbot.Services.Configuration;
using ilvlbot.Services.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace ilvlbot
{
	class Program
	{
		private const string Tag = "main";

		public static IServiceProvider Services;
		internal static int apiInitRetrySeconds = 3;

		private static async Task Main(string[] args)
		{
			// set window title.
			Console.Title = System.Reflection.Assembly.GetExecutingAssembly().FullName;

			// init di
			var services = new ServiceCollection();

			// load settings, logger
			Settings settings = new Settings("settings.conf");
			ILogger logger = new ConsoleLogger();

			// add settings/logger to di
			services.AddSingleton(settings);
			services.AddSingleton(logger);

			// init discord
			var bot = new DiscordBot();
			bot.ConfigureServices(services);
			services.AddSingleton(bot);

			// build service provider!
			Services = services.BuildServiceProvider();

			// init bnet
			while (Api.Initialize(settings.ApiKeys.BattleNet, Services) == false)
			{
				// failed to initialize the bnet api, wait a few seconds and retry.
				logger.Log(Tag, $"Failed to initialize Battle.net API, waiting {apiInitRetrySeconds} seconds before retrying...");
				await Task.Delay(TimeSpan.FromSeconds(apiInitRetrySeconds));
			}

			// grab ctrl+c
			Console.CancelKeyPress += Console_CancelKeyPress;

			// run the bot!
			await bot.Run(Services);
		}

		private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
		{
			Services.GetService<ILogger>()?.Log(Tag, "Console cancel key press received.");
			Services.GetService<DiscordBot>()?.Shutdown().Wait();
		}
	}
}
