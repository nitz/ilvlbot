using core.Services.Logging;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using ilvlbot.Access;
using ilvlbot.Services.Configuration;
using System;
using System.Text;
using System.Threading.Tasks;

namespace ilvlbot.Modules
{
	[Name("Admin")]
	[Group("admin")]
	[RequiredAccessLevel(AccessLevel.BotOwner)]
	public class Administration : ModuleBase<SocketCommandContext>
	{
		private const string Tag = "admin";
		private readonly Settings _settings;
		private readonly ILogger _logger;

		/// <summary>
		/// Constructor to grab the program settings from DI.
		/// </summary>
		/// <param name="settings">The program's settings.</param>
		public Administration(Settings settings, ILogger logger)
		{
			_settings = settings;
			_logger = logger;
		}

		[Command("link"), Alias("invite", "invitelink")]
		[Summary("Has the bot private message you the link to invite him to a server.")]
		[Remarks("Has the bot private message you the link to invite him to a server.")]
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
		[Summary("Changes the 'game' that the bot is playing.")]
		[Remarks("Changes the 'game' that the bot is playing.")]
		[RequiredAccessLevel(AccessLevel.BotOwner)]
		public async Task SetPlaying([Remainder]string game)
		{
			Log($"SetPlaying: {game}");
			await Context.Client.SetGameAsync(game, null, ActivityType.Playing);
		}
		
		[Command("oauth"), Alias("auth", "authorization")]
		[Summary("Gets information about the current bnet authorization.")]
		[Remarks("Will show when the token was aquired, and when it expires, as well as a small slice of it. " +
			"Since this can reveal potentially sensative information, it's only usable in DM.")]
		[RequireContext(ContextType.DM)]
		[RequiredAccessLevel(AccessLevel.BotOwner)]
		public async Task GetOAuthInfo()
		{
			Log("GetOAuthInfo");
			await ReplyAsync(GetOAuthTokenOutputString().ToString());
		}
		
		[Command("oauthrenew"), Alias("renewoauth")]
		[Summary("Forces a renew of the the OAuth token, if possible.")]
		[Remarks("Will attempt to renew the OAuth token, then display information about it. " +
			"Since this can reveal potentially sensative information, it's only usable in DM.")]
		[RequireContext(ContextType.DM)]
		[RequiredAccessLevel(AccessLevel.BotOwner)]
		public async Task GetNewOAuthToken()
		{
			bool renewed = await bnet.Api.RenewOAuthTokenNowAsync();

			if (renewed)
			{
				Log("Renewed OAuth token.");
				var output = new StringBuilder();
				output.AppendLine("OAuth token renewed.");
				output.AppendLine(GetOAuthTokenOutputString());
				await ReplyAsync(output.ToString());
			}
			else
			{
				Log("Failed to renew OAuth token.");
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

			_logger.Log(Tag, $"[Req:{chan}/{Context.Message.Author.Username}#{Context.Message.Author.Discriminator}]: {msg}{s}");
		}
	}
}
