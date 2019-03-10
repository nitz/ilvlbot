using core.Services.Logging;
using Discord;
using Discord.WebSocket;
using ilvlbot.Services.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ilvlbot
{
	public class DiscordBot
	{
		private const string Tag = "bot";
		private const string TagDiscord = "discord";

		private DiscordSocketClient _client;
		private CommandHandler _commands;
		private ILogger _logger;
		private Settings _settings;

		public DiscordBot()
		{
			DiscordSocketConfig config = new DiscordSocketConfig()
			{
				LogLevel = LogSeverity.Info,
			};

			_client = new DiscordSocketClient(config);
		}

		public async Task RunAsync(IServiceProvider services, CancellationToken cancellationToken = default)
		{
			try
			{
				// grab services.
				_logger = services.GetService<ILogger>();
				_settings = services.GetService<Settings>();

				_logger.Log(Tag, "Starting up...");

				_client.Log += DiscordLog;
				_client.Ready += ClientOnReady;
				_client.Connected += ClientOnConnected;

				_logger.Log(Tag, "Connecting...");

				await _client.LoginAsync(TokenType.Bot, _settings.ApiKeys.Discord.Token);
				await _client.StartAsync();

				// install commands.
				await _commands.InitializeCommandModulesAsync(services);

				// hang out forever.
				try
				{
					await Task.Delay(-1, cancellationToken);
				}
				catch (TaskCanceledException)
				{
					// all good!
				}
			}
			catch (System.Net.WebException wex)
			{
				_logger.Log(Tag, $"DiscordBot.Run() failed: {wex.Message}");
			}
		}

		#region Client Event Handlers

		private Task ClientOnConnected()
		{
			_logger.Log(Tag, $"[Connected] Connection: {_client.ConnectionState}, Login: {_client.LoginState}.");
			return Task.CompletedTask;
		}

		private Task ClientOnReady()
		{
			_logger.Log(Tag, $"[Ready] Ready.");
			return Task.CompletedTask;
		}

		private Task DiscordLog(LogMessage l)
		{
			if (l.Exception == null)
			{
				_logger.Log(TagDiscord, $"[{l.Severity}:{l.Source}] {l.Message}");
			}
			else
			{
				_logger.Log(TagDiscord, $"[!:{l.Exception.GetType().Name}] [{l.Severity}:{l.Source}] {l.Exception.Message}; {l.Message}");
			}

			return Task.CompletedTask;
		}

		#endregion Client Event Handlers

		public async Task ShutdownAsync()
		{
			if (_client != null && _client.ConnectionState == ConnectionState.Connected)
			{
				_logger.Log(Tag, "Shutting down...");
				await _client.StopAsync();
				_client = null;
			}
		}

		public async Task SetGameAsync(string game, string url = "")
		{
			await _client.SetGameAsync(game, url, ActivityType.Playing);
			_logger.Log(Tag, $"Game changed to {game} ({url}).");
		}

		public void ConfigureServices(IServiceCollection services)
		{
			// add ourself to our dependency mapper
			services.AddSingleton(_client);

			// create our command handler, and get it's services.
			if (_commands == null)
			{
				_commands = new CommandHandler();
				_commands.ConfigureServices(services);
			}
		}
	}
}
