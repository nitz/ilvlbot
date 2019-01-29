using Discord.Commands;
using Discord.WebSocket;
using ilvlbot.Services.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// modified via https://github.com/Aux/Discord.Net-Example/blob/1.0/Discord.Net-Example/Common/Attributes/MinPermissions.cs
/// </summary>
namespace ilvlbot.Access
{
	/// <summary>
	/// Set the minimum permission required to use a module or command
	/// similar to how MinPermissions works in Discord.Net 0.9.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class RequiredAccessLevelAttribute : PreconditionAttribute
	{
		private readonly AccessLevel _requiredLevel;
		private Settings _settings;

		public override string ErrorMessage { get; set; }

		public RequiredAccessLevelAttribute(AccessLevel requiredLevel)
		{
			_requiredLevel = requiredLevel;
		}

		public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider service)
		{
			// grab settings
			_settings = service.GetService<Settings>();

			// Get the acccesslevel for this context
			var contextAccessLevel = GetAccessLevel(context);

			// If the user's access level is greater than the required level, return success.
			if (contextAccessLevel >= _requiredLevel)
			{
				return Task.FromResult(PreconditionResult.FromSuccess());
			}
			else
			{
				return Task.FromResult(PreconditionResult.FromError(ErrorMessage ?? $"Insufficient permissions. Your access level is {contextAccessLevel}, requires {_requiredLevel}."));
			}
		}

		public AccessLevel GetAccessLevel(ICommandContext c)
		{
			// Prevent other bots from executing commands.
			if (c.User.IsBot)
				return AccessLevel.Blocked;

			// Give configured owners special access.
			if (_settings.Owners.Contains(c.User.Id))
				return AccessLevel.BotOwner;

			// Check if the context is in a guild.
			if (c.User is SocketGuildUser user)
			{
				// Check if the user is the guild owner.
				if (c.Guild.OwnerId == user.Id)
					return AccessLevel.GuildOwner;

				// Check if the user has the administrator permission.
				if (user.GuildPermissions.Administrator)
					return AccessLevel.GuildAdministrator;

				// Check if the user can ban, kick, or manage messages.
				if (user.GuildPermissions.ManageMessages ||
					user.GuildPermissions.BanMembers ||
					user.GuildPermissions.KickMembers)
					return AccessLevel.GuildModerator;
			}

			// If nothing else, return a default permission.
			return AccessLevel.User;
		}
	}
}
