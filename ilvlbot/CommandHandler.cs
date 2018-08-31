using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace ilvlbot
{
	public class CommandHandler
	{
		private CommandService _commands;
		private DiscordSocketClient _client;
		private IServiceCollection _serviceCollection;
		private IServiceProvider _serviceProvider;

		public async Task Install(IServiceCollection serviceCollection)
		{
			// Create Command Service, inject it into Dependency Map
			CommandServiceConfig config = new CommandServiceConfig()
			{
				DefaultRunMode = RunMode.Async
			};

			_commands = new CommandService(config);
			
			_serviceCollection = serviceCollection;
			_serviceCollection.AddSingleton(_commands);

			// load all the command modules
			await _commands.AddModulesAsync(Assembly.GetEntryAssembly());

			// build the service provider
			_serviceProvider = _serviceCollection.BuildServiceProvider();

			// subscribe to the incoming message
			_client = _serviceProvider.GetService(typeof(DiscordSocketClient)) as DiscordSocketClient;
			_client.MessageReceived += HandleCommand;
		}

		public async Task HandleCommand(SocketMessage parameterMessage)
		{
			// Don't handle the command if it is a system message
			var message = parameterMessage as SocketUserMessage;
			if (message == null) return;

			// Mark where the prefix ends and the command begins
			int argPos = 0;
			// Determine if the message has a valid prefix, adjust argPos 
			if (!(message.HasMentionPrefix(_client.CurrentUser, ref argPos) ||
				message.HasStringPrefix(Program.Settings.Prefix, ref argPos))) return;

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
