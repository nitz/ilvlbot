using Discord;
using Discord.Commands;
using ilvlbot.Access;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace ilvlbot.Modules
{
	[Name("Admin")]
	[Group("admin")]
	[RequiredAccessLevel(AccessLevel.BotOwner)]
	public class Administration : ModuleBase<SocketCommandContext>
	{
		[Command("link"), Alias("invite", "invitelink")]
		[Remarks("Has the bot private message you the link to invite him to a server.")]
		[Summary("Has the bot private message you the link to invite him to a server.")]
		[RequiredAccessLevel(AccessLevel.BotOwner)]
		public async Task GetInviteLink()
		{
			string link = $"https://discordapp.com/oauth2/authorize?client_id={Program.Settings.ApiKeys.Discord.ClientId}&scope=bot&permissions=0";
			string message = $"Hey, my invite link is {link}\nThe person who uses this link must have the `Manage Server` role to add me!";
			var dmc = await Context.User.GetOrCreateDMChannelAsync();
			await dmc.SendMessageAsync(message);
		}

		[Command("guilds"), Alias("servers")]
		[Remarks("Displays the guilds the bot is currently a member of.")]
		[Summary("Displays the guilds the bot is currently a member of.")]
		[RequiredAccessLevel(AccessLevel.BotOwner)]
		public async Task GetGuilds()
		{
			var output = new StringBuilder();
			output.AppendLine("I'm a member of: ```");

			foreach (var g in Context.Client.Guilds)
			{
				output.AppendLine($"{g.Name} (Owner: {g.Owner})");
			}

			output.Append("```");

			await ReplyAsync(output.ToString());
		}

		[Command("playing"), Alias("game")]
		[Remarks("Changes the 'game' that the bot is playing.")]
		[Summary("Changes the 'game' that the bot is playing.")]
		[RequiredAccessLevel(AccessLevel.BotOwner)]
		public async Task SetPlaying([Remainder]string game)
		{
			Log($"SetPlaying: {game}");
			await Context.Client.SetGameAsync(game, null, StreamType.NotStreaming);
		}

		/// <summary>
		/// Just a simple wrapper for this class to dump to the console.
		/// </summary>
		/// <param name="s">The text to output.</param>
		/// <param name="show_message">If true, will prepend the command message before the string.</param>
		protected void Log(string s, bool show_message = false)
		{
			var msg = (show_message ? $"({Context.Message.Content}) " : "");

			var chan = Context.Message.Channel?.Name ?? "?";
			if (Context.Message.Channel is SocketGuildChannel sgc)
				chan = $"{sgc.Guild.Name}#{chan}";

			Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] [ADMIN] [Req:{chan}/{Context.Message.Author.Username}#{Context.Message.Author.Discriminator}]: {msg}{s}");
		}

		#region Sandbox testing things... can all be removed.

		[Command("permissions"), Alias("roles")]
		[RequiredAccessLevel(AccessLevel.BotOwner)]
		[Disabled]
		public async Task TestRoles([Remainder]string username = "")
		{
			string result = string.Empty;

			if (string.IsNullOrEmpty(username))
				username = Context.User.Username;

			var permissions = (Context.User as Discord.WebSocket.SocketGuildUser)?.GetPermissions(Context.Channel as IGuildChannel);
			result += $"{username}'s permissions: {permissions}\n";
			var user = (Context.Guild.Users.FirstOrDefault(x => x.Username == username));

			result += string.Join(", ", user?.Roles?.Where(x => x?.Name != "@everyone").Select(r => r?.Name) ?? new string[] {});

			await ReplyAsync(result);
		}

		[Command("params")]
		[Disabled]
		public async Task TestParamSplitting(string part1, string part2, int number, [Remainder]string whatsleft = "")
		{
			await ReplyAsync($"Part 1: {part1}\nPart 2: {part2}\nA number: {number}\nWhat's left: {whatsleft}");
		}

		[Command("overloaded")]
		[Disabled]
		public async Task OverloadedCommand(string test)
		{
			await ReplyAsync($"string {test}");
		}

		[Command("overloaded")]
		[Disabled]
		public async Task OverloadedCommand(int test)
		{
			await ReplyAsync($"int {test}");
		}

		[Command("long")]
		[Disabled]
		public async Task TestLongRunningCommand()
		{
			var task = Task.Run(async () =>
			{
				await Task.Delay(5000);
				await ReplyAsync($"It's been five seconds.");
				await Task.Delay(5000);
				await ReplyAsync($"It's been five more.");
			});

			await ReplyAsync($"Okay: gonna do some waiting!");
		}

		[Group("subs")]
		[Disabled]
		class Subgroup : ModuleBase<SocketCommandContext>
		{
			[Command("test"), Alias("test")]
			[Disabled]
			public async Task TestSubgroup([Remainder]string username = "")
			{
				await ReplyAsync("", embed: new EmbedBuilder().WithDescription("testing.").Build());
			}
		}

		#endregion Sandbox
	}
}
