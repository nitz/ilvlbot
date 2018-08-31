using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bnet;
using Discord;

namespace ilvlbot
{
	class Program
	{
		private static void Main(string[] args) => new Program().Start(args).GetAwaiter().GetResult();

		internal static Configuration.Settings Settings;
		internal static DiscordBot Bot;

		static Program()
		{
			Settings = Configuration.Settings.Load("settings.conf");
		}

		private async Task Start(string[] args)
		{
			// set window title.
			Console.Title = System.Reflection.Assembly.GetExecutingAssembly().FullName;

			// init bnet & discord
			Api.Initialize(Settings.ApiKeys.BattleNet);
			Bot = new DiscordBot();

			// grab ctrl+c
			Console.CancelKeyPress += Console_CancelKeyPress;

			// run the bot!
			await Bot.Run();
		}

		private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
		{
			Bot?.Shutdown().GetAwaiter().GetResult();
		}
	}
}
