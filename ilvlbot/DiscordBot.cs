using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace ilvlbot
{
	public class DiscordBot
	{
		private DiscordSocketClient client;
		private CommandHandler commands;

		public DiscordBot()
		{

		}

		public async Task Run()
		{
			try
			{
				Log("Starting up...");

				DiscordSocketConfig config = new DiscordSocketConfig()
				{
					LogLevel = LogSeverity.Info,
				};

				client = new DiscordSocketClient(config);

				client.Log += Log;
				client.Ready += ClientOnReady;
				client.Connected += ClientOnConnected;

				Log("Connecting...");

				await client.LoginAsync(TokenType.Bot, Program.Settings.ApiKeys.Discord.Token);
				await client.StartAsync();

				// add ourself to our dependency mapper
				var serviceCollection = new ServiceCollection();
				serviceCollection.AddSingleton(client);

				// install our commands
				commands = new CommandHandler();
				await commands.Install(serviceCollection);

				// hang out forever.
				await Task.Delay(-1);
			}
			catch (System.Net.WebException wex)
			{
				Log($"DiscordBot.Run() failed: {wex.Message}");
			}
		}

		private Task ClientOnConnected()
		{
			Log($"[Connected] Connection: {client.ConnectionState}, Login: {client.LoginState}.");
			return Task.CompletedTask;
		}

		private Task ClientOnReady()
		{
			Log($"[Ready]");
			return Task.CompletedTask;
		}

		public async Task Shutdown()
		{
			if (client != null && client.ConnectionState == ConnectionState.Connected)
			{
				Log("Disconnecting...");
				await client.StopAsync();
				client = null;
			}
		}

		public async Task SetGame(string game, string url = "")
		{
			await client.SetGameAsync(game, url, StreamType.NotStreaming);
			Log($"Game changed to {game} ({url}).");
		}

		private void Log(string s)
		{
			Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] [BOT] {s}");
		}

		private async Task Log(LogMessage l)
		{
			await Task.Run(() =>
			{
				Log($"[{l.Severity}:{l.Source}] {l.Message}");
			});
		}
	}
}
