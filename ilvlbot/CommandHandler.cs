using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.DependencyInjection;
using ilvlbot.Services.Configuration;

namespace ilvlbot
{
	public class CommandHandler
	{
		private bool _initialized = false;
		private CommandService _commands;
		private DiscordSocketClient _client;
		private IServiceProvider _serviceProvider;
		private Settings _settings;

		public void ConfigureServices(IServiceCollection serviceCollection)
		{
			// Create Command Service, inject it into Dependency Map
			CommandServiceConfig config = new CommandServiceConfig()
			{
				DefaultRunMode = RunMode.Async
			};

			_commands = new CommandService(config);
			serviceCollection.AddSingleton(_commands);
		}

		public async Task InitializeCommandModulesAsync(IServiceProvider provider)
		{
			if (_initialized)
			{
				return;
			}

			_initialized = true;

			// grab the service provider
			_serviceProvider = provider;

			// load all the command modules
			await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);

			// subscribe to the incoming message
			_client = _serviceProvider.GetService(typeof(DiscordSocketClient)) as DiscordSocketClient;
			_client.MessageReceived += HandleCommand;

			// grab settings
			_settings = _serviceProvider.GetService<Settings>();
		}

		public async Task HandleCommand(SocketMessage parameterMessage)
		{
			// Don't handle the command if it is a system message
			if (!(parameterMessage is SocketUserMessage message))
			{
				return;
			}

			// Mark where the prefix ends and the command begins
			int argPos = 0;
			// Determine if the message has a valid prefix, adjust argPos 
			if (!(message.HasMentionPrefix(_client.CurrentUser, ref argPos) ||
				message.HasStringPrefix(_settings.Prefix, ref argPos)))
			{
				return;
			}

			// Create a Command Context
			var context = new SocketCommandContext(_client, message);
			// Execute the Command, store the result
			var result = await _commands.ExecuteAsync(context, argPos, _serviceProvider);

			// If the command failed, notify the user
			if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
				await message.Channel.SendMessageAsync($"**Error:** {result.ErrorReason}");
		}
	}
}
