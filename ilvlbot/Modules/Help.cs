using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;
using ilvlbot.Extensions;
using ilvlbot.Services.Configuration;
using System;

/// <summary>
/// Adapted from https://github.com/Aux/Discord.Net-Example/blob/1.0/Discord.Net-Example/Modules/HelpModule.cs
/// </summary>
namespace ilvlbot.Modules
{
	[Name("Help")]
	public class Help : ModuleBase<SocketCommandContext>
	{
		private CommandService _commandService;
		private Settings _settings;
		private IServiceProvider _services;

		/// <summary>
		/// The changelog values returned by the `changelog` commmand.
		/// </summary>
		private readonly string[] _changelog =
		{
			"```asciidoc",
			"v1.6:: Moved to .NET Core 2.2 (from .NET Framework 4.7.2), restructured DI.",
			"v1.5:: Added alternative server specification syntax for ilvl commands, gilvl ouput refreshed.",
			"v1.4:: Bumped Discord.Net to 2.0.0.",
			"v1.3:: Swapped to using new Blizzard API with OAuth.",
			"v1.2:: Added support for Azerite items, removed artifact/legendary info. Fixed profile images.",
			"v1.1:: Fixed broken WoWToken.info commands, updated dependences.",
			"v1.0:: Initial release.",
			"```"
		};

		/// <summary>
		/// A short information paragraph returned by the `about` command.
		/// </summary>
		private readonly string[] _about =
		{
			"Hi, I was created by <@116026533688115204>.",
			"I'm just a silly little bot that looks up player's (and guild's) item levels via the Battle.Net API.",
			"I'm definitely not perfect, so please do feel free to share issues or ideas you have with my creator!",
			"If you'd like, you can check out my source code at <https://github.com/nitz/ilvlbot>!"
		};

		/// <summary>
		/// Constructor to grab the program settings from DI.
		/// </summary>
		/// <param name="settings">The program's settings.</param>
		public Help(CommandService commandService, Settings settings, IServiceProvider services)
		{
			_commandService = commandService;
			_settings = settings;
			_services = services;
		}

		[Command("help")]
		[Summary("This command!")]
		[Remarks("Really?")]
		public async Task HelpCommand([Remainder]string command = "")
		{
			if (string.IsNullOrEmpty(command))
				await HelpGeneral();
			else
				await HelpCommandInfo(command);
		}

		/// <summary>
		/// Spit out info about all the commands I have, that the user has access to.
		/// </summary>
		public async Task HelpGeneral()
		{
			string prefix = _settings.Prefix;
			var builder = new EmbedBuilder()
			{
				Color = new Color(114, 137, 218),
				Description = "These are the commands you can use.\nTry `!help <command>` for more information."
			};

			foreach (var module in _commandService.Modules)
			{
				string description = null;
				foreach (var cmd in module.Commands)
				{
					var result = await cmd.CheckPreconditionsAsync(Context, _services);
					if (result.IsSuccess)
					{
						description += $"{prefix}{cmd.Aliases.First()}";
						if (cmd.Summary?.Length > 0)
							description += " — " + cmd.Summary;
						description += "\n";
					}
				}

				if (!string.IsNullOrWhiteSpace(description))
				{
					builder.AddField(x =>
					{
						x.Name = module.Name;
						x.Value = description;
						x.IsInline = false;
					});
				}
			}

			await ReplyAsync("", false, builder.Build());
		}
		
		/// <summary>
		/// Spit out information about the specifically requested command.
		/// </summary>
		/// <param name="command">The command to get info about</param>
		public async Task HelpCommandInfo(string command)
		{
			var result = _commandService.Search(Context, command);

			if (!result.IsSuccess)
			{
				await ReplyAsync($"Sorry, I couldn't find a command like **{command}**.");
				return;
			}

			string prefix = _settings.Prefix;
			var builder = new EmbedBuilder()
			{
				Color = new Color(114, 137, 218),
				Description = $"Here are some commands like **{command}**"
			};

			foreach (var match in result.Commands)
			{
				var cmd = match.Command;

				builder.AddField(x =>
				{
					x.Name = string.Join(", ", cmd.Aliases);
					x.Value = $"Parameters: {string.Join(", ", cmd.Parameters.Select(p => p.Name))}\n" +
							  $"Remarks: {cmd.Remarks}";
					x.IsInline = false;
				});
			}

			await ReplyAsync("", false, builder.Build());
		}

		[Command("about"), Alias("info")]
		[Summary("Information about this bot.")]
		[Remarks("Returns a little bit of information about myself!")]
		public async Task About()
		{
			string about = string.Join("\n", _about) +
							"\n" + GenerateTechnicalInfoForNerds();

			await ReplyAsync(about);
		}

		[Command("changelog"), Alias("changes")]
		[Summary("What's new?")]
		[Remarks("Returns a little bit of information about what's changed lately.")]
		public async Task Changelog()
		{
			string changelog = string.Join("\n", _changelog) +
							"\n" + GenerateTechnicalInfoForNerds();

			await ReplyAsync(changelog);
		}

		/// <summary>
		/// Generate a small code block that describes our application and the version of discord.net we're using.
		/// </summary>
		/// <returns>The description string</returns>
		private string GenerateTechnicalInfoForNerds()
		{
			string appFullName = AssemblyExtensions.FullName;
			var buildDate = AssemblyExtensions.CompileDate;
			string builtOn = buildDate.ToLongDateString() + " at " + buildDate.ToLongTimeString();
			string discordNet = ($"v{DiscordConfig.Version} (API v{DiscordConfig.APIVersion}); ") + System.Reflection.Assembly.GetAssembly(Context.Client.GetType()).FullName;
			return "Technical info for nerds:\n" +
					$"```" +
					$"Application: {appFullName}\n" +
					$"Discord.Net: {discordNet}\n" +
					$"Built on: {builtOn}" +
					$"```";
		}
	}
}
