using bnet.Requests;
using bnet.Responses;
using core.Services.Logging;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using ilvlbot.Services.Configuration;
using ilvlbot.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ilvlbot.Modules
{
	[Name("Item Level / Armory")]
	public partial class ItemLevel : ModuleBase<SocketCommandContext>
	{
		// these settings should be promoted to the settings file
		private bool reportItemLevel = true;
		private bool reportAzeriteLevel = true;
		private bool reportArtifactLevel = false;
		private bool reportLegendaries = false;
		private bool reportAchievementPoints = true;

		/// <summary>
		/// A handful of reasons why I might ignore a command ;)
		/// </summary>
		private static string[] lazyReasons =
		{
			"You want me to do TWO things?!",
			"Nuh uh. Don' wanna.",
			"But I just did it!",
			"But I'm tired...",
			"Eh, I'll do it later.",
			"Nah.",
			"Bite my shiny metal ass."
		};

		/// <summary>
		/// A handful of complaints about people we might be looking up ;)
		/// </summary>
		private static readonly string[] judgementalLines =
		{
			"Eh, they look okay to me.",
			"Seriously, in *that* gear?",
			"I guess if we needed a pug...",
			"They probably stand in the fire.",
			"Okay, but they can't roll on my gear.",
			"I bet they bought all their gear!",
			"Did they get all their gear from world quests?!"
		};

		/// <summary>
		/// The mapping of realm/guild names to the latest data for each of those.
		/// Static, because this information isn't unique to any one instance of this module.
		/// </summary>
		private static Dictionary<string, GuildInfo> info = new Dictionary<string, GuildInfo>();

		/// <summary>
		/// Item Level Bot specific settings
		/// </summary>
		private ItemLevelBotConfig _config;

		/// <summary>
		/// A logger.
		/// </summary>
		private ILogger _logger;

		/// <summary>
		/// Constructor to grab the program settings from DI.
		/// </summary>
		/// <param name="settings">The program's settings.</param>
		public ItemLevel(Settings settings, ILogger logger)
		{
			_config = settings.ItemLevel;
			_logger = logger;
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

			_logger.Log("ilvl", $"[Req:{chan}/{Context.Message.Author.Username}#{Context.Message.Author.Discriminator}]: {msg}{s}");
		}

		/// <summary>
		/// Command: Gets the item level and other information about the given character.
		/// </summary>
		/// <param name="character">The name of the character to look up. (Or realm and character as "Realm Name/Character")</param>
		/// <returns>After a task, nothing.</returns>
		[Command("ilvl"), Alias("itemlvl", "itemlevel", "armory")]
		[Remarks("Gets a character's item level and more. Character parameter can be passed in the form `Realm Name/Character` to look up characters on a realm other than the default.")]
		[Summary("Gets a character's item level and more.")]
		public async Task CharacterItemLevel([Remainder]string character = "")
		{
			try
			{
				var cmd = ParsedCommand.Parse(character, _config.DefaultRealm, string.Empty);
				var output = await ProcessCharacterCommand(cmd);

				if (output.IsEmbedResult)
					await ReplyAsync("", embed: output.EmbedResult);
				else
					await ReplyAsync(output.StringResult);
			}
			catch (Exception ex)
			{
				string output = $"I couldn't process your command. Tell a nerd: {ex.Message} ({ex.GetCallingSite()})";
				Log(output, true);
				await ReplyAsync(output);
			}
		}

		/// <summary>
		/// Command: Gets the item levels of the top members of the given guild.
		/// </summary>
		/// <param name="text">The remainder of the command from the handler.</param>
		/// <returns>After a task, nothing.</returns>
		[Command("gilvl"), Alias("gitemlvl", "gitemlevel", "guilditemlevel")]
		[Remarks("Gets the item level for a given guild. Character parameter can be passed in the form `Realm Name/Guild` to look up characters on a realm other than the default.")]
		[Summary("Gets the item level for a given guild.")]
		public async Task GuildItemLevel([Remainder]string guild = "")
		{
			try
			{
				var cmd = ParsedCommand.Parse(guild, _config.DefaultRealm, _config.DefaultGuild);
				var output = await ProcessGuildCommand(cmd);

				if (output.IsEmbedResult)
					await ReplyAsync("", embed: output.EmbedResult);
				else
					await ReplyAsync(output.StringResult);
			}
			catch (Exception ex)
			{
				string output = $"I couldn't process your command. Tell a nerd: {ex.Message} ({ex.GetCallingSite()})";
				Log(output, true);
				await ReplyAsync(output);
			}
		}

		/// <summary>
		/// Processes a character command.
		/// </summary>
		/// <param name="cmd">The command to process</param>
		/// <returns>A CharacterResult which will have either a string to reply to the channel with, or an Embed to send to the channel.</returns>
		private async Task<CommandResult> ProcessCharacterCommand(ParsedCommand cmd)
		{
			// we need a target name for a character!
			if (cmd.TargetName == string.Empty)
			{
				Log("No target.", true);
				return new CommandResult($"I can't look up a character if you don't give me a name!");
			}

			Log($"ProcessCharacterCommand: {cmd.RealmName}/{cmd.TargetName}");

			// tell them i'm 'typing' whlie I'm looking up the info.
			await Context.Channel.TriggerTypingAsync();

			var character_result = await Get.CharacterInfo(cmd.RealmName, cmd.TargetName, bnet.Requests.Fields.Character.Full);

			if (character_result.Successful == false)
			{
				if (character_result.StatusCode == System.Net.HttpStatusCode.NotFound)
					return new CommandResult($"I couldn't find `{cmd.RealmName}/{cmd.TargetName}`. Perhaps they're too low level or haven't logged in recently?");
				return new CommandResult($"I had problems finding infomation for `{cmd.RealmName}/{cmd.TargetName}`, sorry! Tell a nerd: {character_result.ErrorText}");
			}

			var character = character_result.Result;

			return new CommandResult(await BuildCharacterEmbed(character));

		}

		/// <summary>
		/// Builds a Discord Embed object to return that describes the character given.
		/// </summary>
		/// <param name="fc">The Battle.Net character to build the embed for.</param>
		/// <returns>The newly created embed.</returns>
		private async Task<Embed> BuildCharacterEmbed(Character fc)
		{
			// some helper strings.
			const string not_applicable = "*N/A*";
			const string wowprogress_base = "https://www.wowprogress.com/character/us";
			bool male = fc.gender == 0;
			string he_she = male ? "He" : "She";
			//string his_her = male ? "His" : "Her";
			//string leg_s = fc.items.legendaryItemCount == 1 ? "" : "s";

			// use title/url as wowprogress link
			string char_title = $"{fc.name} on {fc.realm}";

			// the 'one line' description
			string description = $"{fc.raceSide.ToDiscordEmoji()}{fc.className.ToDiscordEmoji()}{he_she} is a level {fc.level} {fc.raceSide} {fc.raceName} {fc.specClass}.\n";

			// add the guild info if applicable.
			if (fc.guild != null)
				description += $"\n{he_she} is one of <**{fc.guild.name}**>'s {fc.guild.members} members.\n";

			// some urls to use
			string armory_url = fc.armoryUrl;
			string avatar_url = fc.avatarUrl;
			string inset_url = fc.insetImageUrl;
			string profile_image_url = fc.profileImageUrl;
			string wowprogress_title = $"View {fc.name} on WoWProgress";
			string wowprogress_url = $"{wowprogress_base}/{fc.realm.ToRealmUri()}/{fc.name}";

			// actual fields
			string ilvl = $"**{fc.items.calculatedItemLevel:0.00}** ({fc.items.averageItemLevel} bags)";
			string azerite_level = (fc.items?.neck?.isAzeriteItem ?? false) ? $"**{fc.items.neck.azeriteLevel}** (ilvl {fc.items.neck.itemLevel})" : not_applicable;
			string artifact_level = (fc.items?.artifactRank ?? 0) > 0 ? $"**{fc.items.artifactRank}**" : not_applicable;
			string legendary_equipped = (fc.items?.legendaryItemCount ?? 0) > 0 ? $"**{fc.items.legendaryItemCount}**" : "0";
			string ach_points = $"**{fc.achievementPoints}**";
			
			List<EmbedFieldBuilder> ach_fields = new List<EmbedFieldBuilder>();

			// check achievements
			foreach (int achi_id in _config.CheckedAchievements)
			{
				try
				{
					var ach = (await Get.Achievement(achi_id))?.Result;
					bool character_has_completed = fc.achievements.achievementsCompleted.Contains(achi_id);
					string completed_str = character_has_completed ? ":white_check_mark:" : ":x:";
					ach_fields.Add(new EmbedFieldBuilder() { IsInline = true, Name = ach.title, Value = completed_str});
				}
				catch
				{
					ach_fields.Add(new EmbedFieldBuilder() { IsInline = true, Name = $"{achi_id}", Value = ":question:" });
				}
			}

			// build the 'author' as the player.
			EmbedAuthorBuilder eab = new EmbedAuthorBuilder();
			eab.Name = char_title;
			eab.IconUrl = avatar_url;
			eab.Url = armory_url;

			// buld the 'footer' as something funny
			EmbedFooterBuilder efb = new EmbedFooterBuilder();
			//efb.IconUrl = "https://us.battle.net/forums/static/images/avatars/wow/avatar-wow-default.png";
			efb.Text = judgementalLines.GetRandom();

			// the last modified is (usually) the last time the character logged out of the game.
			DateTimeOffset dto = DateTime.SpecifyKind(fc.lastModifiedDateTime, DateTimeKind.Local);

			// build the Embed
			EmbedBuilder builder = new EmbedBuilder()
			{
				// title/url for wowprogress
				Title = wowprogress_title,
				Url = wowprogress_url,

				// some general pretty
				Color = fc.classInfo.color.ToDiscordColor(),
				ThumbnailUrl = avatar_url,
				ImageUrl = inset_url,

				// description for the 'one liner'
				Description = description,

				// arthor for the armory link/tiny image
				Author = eab,

				// footer to be silly
				Footer = efb,

				// and the date/time.
				Timestamp = dto,
			};

			// add all the fields
			if (reportItemLevel) builder.AddField(new EmbedFieldBuilder() { IsInline = true, Name = "Item Level", Value = ilvl });
			if (reportAzeriteLevel) builder.AddField(new EmbedFieldBuilder() { IsInline = true, Name = "Azerite Level", Value = azerite_level });
			if (reportArtifactLevel) builder.AddField(new EmbedFieldBuilder() { IsInline = true, Name = "Artifact Level", Value = artifact_level });
			if (reportLegendaries) builder.AddField(new EmbedFieldBuilder() { IsInline = true, Name = "Legendaries", Value = legendary_equipped });
			if (reportAchievementPoints) builder.AddField(new EmbedFieldBuilder() { IsInline = true, Name = "Achievement Points", Value = ach_points });

			// and the acheivement fields
			foreach (var b in ach_fields)
				builder.AddField(b);

			return builder.Build();
		}

		/// <summary>
		/// Processes a guild command.
		/// </summary>
		/// <param name="cmd">The command to process.</param>
		/// <returns>A CharacterResult representing the itemlevels of the top guild members,
		/// which will have either a string to reply to the channel with, or an Embed to send to the channel.</returns>
		private async Task<CommandResult> ProcessGuildCommand(ParsedCommand cmd)
		{
			GuildInfo g = GetGuild(cmd.RealmName, cmd.TargetName, _config.GuildTargetLevel);
			TimeSpan time_since_update = DateTime.Now - g.LastRefresh;

			if (time_since_update > _config.GuildRequestCooldown)
			{
				Log($"ProcessGuildCommand: `{cmd.RealmName}/{cmd.TargetName}`");
				var holdOnMessage = await ReplyAsync($"Okay, give me a few. Looking up `{cmd.TargetName}` on `{cmd.RealmName}` now.");
				var output_task = UpdateGuild(g);

				// wait for the guild to update.
				while (output_task.IsCompleted == false)
				{
					// tell them i'm 'typing', which lasts for 10 seconds, which is almost how long i'll wait to see if the task is done.
					// if it's not, I'll send typing again.
					await Context.Channel.TriggerTypingAsync();
					output_task.Wait(TimeSpan.FromSeconds(8));
				}

				// done doing my thing, remove my 'hold on' message.
				await holdOnMessage.DeleteAsync();

				return output_task.Result;
			}
			else
			{
				TimeSpan cooldown_left = _config.GuildRequestCooldown - time_since_update;

				// give em a lazy reason.
				string output = $"{lazyReasons.GetRandom()} (cooldown remaining: {cooldown_left:mm\\:ss})";
				Log($"Not updating guild: {output}", true);

				return new CommandResult(output);
			}
		}
		
		/// <summary>
		/// Updates the information for an entire guild on realm.
		/// </summary>
		/// <param name="guild">The guild to update.</param>
		/// <returns>Output string designed to be sent to the channel.</returns>
		private async Task<CommandResult> UpdateGuild(GuildInfo guild)
		{
			// time how long it takes me to get everyone.
			var sw = new System.Diagnostics.Stopwatch();
			sw.Start();

			// refresh and report
			bool refresh_successful = await guild.Refresh(s => Log(s));

			if (refresh_successful)
			{
				// build output string
				StringBuilder sb = new StringBuilder();
				sb.AppendLine($"`{guild.Guild}` has {guild.CharacterCount} characters, {guild.TargetLevelCharacterCount} are level {guild.TargetLevel}.");

				if (guild.TargetLevelCharacterCount == 0)
				{
					sb.AppendLine($"Since none are {guild.TargetLevel}, I won't bother looking up their item levels. :wink:");
					return new CommandResult(sb.ToString());
				}

				int number_shown = Math.Min(guild.GuildMembers.Count, _config.MaxGuildCharacters);

				sb.AppendLine($"Here's the top {number_shown} ('in bags' ilvl in parenthesis)");

				// asciidoc code blocks have some nice syntax highlighting I can abuse.
				// lines starting "<text> :: <text>" will have the first text and double colon turned red.
				// lines with a line of dashes or equals under them will be blue (with the dashed line)
				// lines with text like "[<text>]" will be red, as will ":<text>:"
				// lines starting with * or - will have the * or - colored red.
				// some basic markdown shows too, (without eating characters) which is neat.
				sb.AppendLine("```asciidoc"); 

				sb.AppendLine($"{guild.Guild}");
				sb.AppendLine("-----");

				// include the top maxMembers characters
				int count = 0;
				foreach (var c in guild.GuildMembers)
				{
					sb.AppendLine($"{c.character.items.calculatedItemLevel:0.00} ({c.character.items.averageItemLevel}) :: {c.guildCharacter.name}");

					++count;
					if (count >= _config.MaxGuildCharacters)
						break;
				}

				// let them know about anyone I failed to get information on.
				if (guild.FailedCharacters.Count > 0)
				{
					sb.AppendLine("");
					sb.Append("I had a problem getting info for: ");
					foreach (var f in guild.FailedCharacters)
						sb.Append(f + ", ");

					sb.Remove(sb.Length - 2, 2);
				}

				// close the code block.
				sb.AppendLine("```");

				var secondsS = (int)sw.Elapsed.TotalSeconds != 1 ? "s" : "";
				sb.AppendLine($"It took me {sw.Elapsed.TotalSeconds:0} second{secondsS} to look everyone up.");

				// build a nice embed to wrap it all up in.
				EmbedFooterBuilder efb = new EmbedFooterBuilder()
					.WithText(AssemblyExtensions.ShortName);

				DateTimeOffset dto = DateTime.SpecifyKind(guild.LastRefresh, DateTimeKind.Local);

				EmbedBuilder builder = new EmbedBuilder()
				{
					// title/url for wowprogress
					Title = $"{guild.Guild} Item Level Report",
					//Url = "maybe get an armory url here...",

					// some general pretty
					Color = Color.DarkPurple,

					// description is my primary output
					Description = sb.ToString(),

					// footer for timestamp and appname
					Footer = efb,

					// and the date/time.
					Timestamp = dto,
				};

				return new CommandResult(builder.Build());
			}

			// something went wrong, report the error.
			return new CommandResult($"There was an issue getting the info: {guild.LastError}");
		}

		/// <summary>
		/// Returns a guild from the info structure based on the
		/// realm and guild as a key. If one doesn't exist,
		/// creates a new one and returns that.
		/// </summary>
		/// <param name="realm">The realm of the guild to return.</param>
		/// <param name="guild">The name of the guild on 'realm' to return.</param>
		/// <returns></returns>
		private GuildInfo GetGuild(string realm, string guild, int target_level)
		{
			string key = GuildInfo.MakeKey(realm, guild);
			if (info.ContainsKey(key))
				return info[key];

			info[key] = new GuildInfo(realm, guild, target_level);
			return info[key];
		}
	}
}
