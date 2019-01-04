using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;
using ilvlbot.Extensions;

/// <summary>
/// Adapted from https://github.com/Aux/Discord.Net-Example/blob/1.0/Discord.Net-Example/Modules/HelpModule.cs
/// </summary>
namespace ilvlbot.Modules
{
	[Name("Help")]
	public class Help : ModuleBase<SocketCommandContext>
	{
		private CommandService _service;

		// Create a constructor for the commandservice dependency
		public Help(CommandService service)
		{
			_service = service;
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

		public async Task HelpGeneral()
		{
			string prefix = Program.Settings.Prefix;
			var builder = new EmbedBuilder()
			{
				Color = new Color(114, 137, 218),
				Description = "These are the commands you can use.\nTry `!help <command>` for more information."
			};

			foreach (var module in _service.Modules)
			{
				string description = null;
				foreach (var cmd in module.Commands)
				{
					var result = await cmd.CheckPreconditionsAsync(Context);
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
		
		public async Task HelpCommandInfo(string command)
		{
			var result = _service.Search(Context, command);

			if (!result.IsSuccess)
			{
				await ReplyAsync($"Sorry, I couldn't find a command like **{command}**.");
				return;
			}

			string prefix = Program.Settings.Prefix;
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
			var asm = System.Reflection.Assembly.GetExecutingAssembly();
			string app = asm.FullName;
			var bd = AssemblyExtensions.CompileDate;
			string built = bd.ToLongDateString() + " at " + bd.ToLongTimeString();
			string discordnet = ($"v{DiscordConfig.Version} (API v{DiscordConfig.APIVersion}); ") + System.Reflection.Assembly.GetAssembly(Context.Client.GetType()).FullName;
			string about = "Hi, I was created by <@116026533688115204>.\n" +
							"I'm just a silly little bot that looks up player's (and guild's) item levels via the Battle.Net API.\n" +
							"I'm definitely not perfect, so please do feel free to share issues or ideas you have with my creator!\n" +
							"If you'd like, you can check out my source code at <https://github.com/nitz/ilvlbot>!\n" +
							"\nTechnical info for nerds:\n" +
							$"```" +
							$"Application: {app}\n" +
							$"Discord.Net: {discordnet}\n" +
							$"Built on: {built}" +
							$"```";

			await ReplyAsync(about);
		}

		[Command("changelog"), Alias("changes")]
		[Summary("What's new?")]
		[Remarks("Returns a little bit of information about what's changed lately.")]
		public async Task Changelog()
		{
			var asm = System.Reflection.Assembly.GetExecutingAssembly();
			string app = asm.FullName;
			var bd = AssemblyExtensions.CompileDate;
			string built = bd.ToLongDateString() + " at " + bd.ToLongTimeString();
			string discordnet = ($"v{DiscordConfig.Version} (API v{DiscordConfig.APIVersion}); ") + System.Reflection.Assembly.GetAssembly(Context.Client.GetType()).FullName;
			string about = "v1.4: Bumped Discord.Net to 2.0.0.\n" + 
							"v1.3: Swapped to using new Blizzard API with OAuth.\n" +
							"v1.2: Added support for Azerite items, removed artifact/legendary info. Fixed profile images.\n" +
							"v1.1: Fixed broken WoWToken.info commands, updated dependances. \n" +
							"v1.0: Initial release." +
							"\nTechnical info for nerds:\n" +
							$"```" +
							$"Application: {app}\n" +
							$"Discord.Net: {discordnet}\n" +
							$"Built on: {built}" +
							$"```";

			await ReplyAsync(about);
		}
	}
}
