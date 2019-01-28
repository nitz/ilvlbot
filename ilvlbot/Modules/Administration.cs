using Discord;
using Discord.Commands;
using ilvlbot.Access;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using ilvlbot.Services.Configuration;

namespace ilvlbot.Modules
{
	[Name("Admin")]
	[Group("admin")]
	[RequiredAccessLevel(AccessLevel.BotOwner)]
	public class Administration : ModuleBase<SocketCommandContext>
	{
		private Settings _settings;

		/// <summary>
		/// Constructor to grab the program settings from DI.
		/// </summary>
		/// <param name="settings">The program's settings.</param>
		public Administration(Settings settings)
		{
			_settings = settings;
		}

		[Command("link"), Alias("invite", "invitelink")]
		[Remarks("Has the bot private message you the link to invite him to a server.")]
		[Summary("Has the bot private message you the link to invite him to a server.")]
		[RequiredAccessLevel(AccessLevel.BotOwner)]
		public async Task GetInviteLink()
		{
			string link = $"https://discordapp.com/oauth2/authorize?client_id={_settings.ApiKeys.Discord.ClientId}&scope=bot&permissions=0";
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
			await Context.Client.SetGameAsync(game, null, ActivityType.Playing);
		}
		
		[Command("oauth"), Alias("auth", "authentication")]
		[Remarks("Gets information about the current bnet authentication.")]
		[Summary("Gets information about the current bnet authentication.")]
		public async Task GetOAuthInfo()
		{
			await ReplyAsync(GetOAuthTokenOutputString().ToString());
		}
		
		[Command("oauthrenew"), Alias("renewoauth")]
		[Remarks("Forces a renew of the the OAuth token, if possible.")]
		[Summary("Forces a renew of the the OAuth token, if possible.")]
		public async Task GetNewOAuthToken()
		{
			bool renewed = await bnet.Api.RenewOAuthTokenNowAsync();

			if (renewed)
			{
				var output = new StringBuilder();
				output.AppendLine("OAuth token renewed.");
				output.AppendLine(GetOAuthTokenOutputString());
				await ReplyAsync(output.ToString());
			}
			else
			{
				await ReplyAsync("Failed to renew OAuth token.");
			}
		}

		/// <summary>
		/// Gets information about the current OAuth info.
		/// </summary>
		/// <returns>A string representing the current authorization info.</returns>
		private string GetOAuthTokenOutputString()
		{
			var (tokenSlice, createdAt, expiresAt) = bnet.Api.GetCurrentOAuthInfo();
			var output = new StringBuilder();
			output.AppendLine("Authorization information: ```");
			output.AppendLine($"Token Slice: {tokenSlice}");
			output.AppendLine($"Aquired: {createdAt}");
			output.AppendLine($"Expires: {expiresAt}");
			output.Append("```");
			return output.ToString();
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
	}
}
