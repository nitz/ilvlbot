using bnet;
using core.Services.Logging;
using ilvlbot.Services.Configuration;
using ilvlbot.Services.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ilvlbot
{
	class Program
	{
		internal static IServiceProvider Services;
		internal static int apiInitRetrySeconds = 3;

		private const string Tag = "main";
		private const string SettingsFile = "settings.conf";
		private static readonly CancellationTokenSource botCancellationTokenSource = new CancellationTokenSource();

		private static async Task Main(string[] args)
		{
			// set window title.
			Console.Title = System.Reflection.Assembly.GetExecutingAssembly().FullName;

			// init di
			var services = new ServiceCollection();

			// load settings, logger
			Settings settings = new Settings(SettingsFile);
			ILogger logger = new ConsoleLogger();

			// make sure api keys are set.
			if (settings.ApiKeys.KeysSet == false)
			{
				logger.Log(Tag, "With missing API keys, the application can't run.\nPress any key to exit.");
				Console.ReadKey();
				Environment.Exit(-1);
			}

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
			while (await Api.InitializeAsync(settings.ApiKeys.BattleNet, Services) == false)
			{
				// failed to initialize the bnet api, wait a few seconds and retry.
				logger.Log(Tag, $"Failed to initialize Battle.net API, waiting {apiInitRetrySeconds} seconds before retrying...");
				await Task.Delay(TimeSpan.FromSeconds(apiInitRetrySeconds));
			}

			// grab ctrl+c
			Console.CancelKeyPress += Console_CancelKeyPress;

			// run the bot!
			await bot.RunAsync(Services, botCancellationTokenSource.Token);

			// shut down blocking.
			await bot.ShutdownAsync();
			await Api.ShutdownAsync();

			// give things a few ms to finish up.
			Task.Delay(500).Wait();
		}

		private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
		{
			Services.GetService<ILogger>()?.Log(Tag, "Console cancel key press received.");
			botCancellationTokenSource.Cancel();
		}
	}
}
