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
		private DiscordSocketClient _client;
		private CommandHandler _commands;

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

				_client = new DiscordSocketClient(config);

				_client.Log += Log;
				_client.Ready += ClientOnReady;
				_client.Connected += ClientOnConnected;

				Log("Connecting...");

				await _client.LoginAsync(TokenType.Bot, Program.Settings.ApiKeys.Discord.Token);
				await _client.StartAsync();

				// add ourself to our dependency mapper
				var serviceCollection = new ServiceCollection();
				serviceCollection.AddSingleton(_client);

				// install our commands
				_commands = new CommandHandler();
				await _commands.Install(serviceCollection);

				// hang out forever.
				await Task.Delay(-1);
			}
			catch (System.Net.WebException wex)
			{
				Log($"DiscordBot.Run() failed: {wex.Message}");
			}
		}

		#region Client Event Handlers

		private Task ClientOnConnected()
		{
			Log($"[Connected] Connection: {_client.ConnectionState}, Login: {_client.LoginState}.");
			return Task.CompletedTask;
		}

		private Task ClientOnReady()
		{
			Log($"[Ready]");
			return Task.CompletedTask;
		}

		private Task Log(LogMessage l)
		{
			Log($"[{l.Severity}:{l.Source}] {l.Message}");
			return Task.CompletedTask;
		}

		#endregion Client Event Handlers

		public async Task Shutdown()
		{
			if (_client != null && _client.ConnectionState == ConnectionState.Connected)
			{
				Log("Disconnecting...");
				await _client.StopAsync();
				_client = null;
			}
		}

		public async Task SetGame(string game, string url = "")
		{
			await _client.SetGameAsync(game, url, StreamType.NotStreaming);
			Log($"Game changed to {game} ({url}).");
		}

		private void Log(string s)
		{
			Task.Run(() =>
			{
				Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] [BOT] {s}");
			});
		}
	}
}
