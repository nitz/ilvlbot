using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using ilvlbot.Extensions;
using Discord.Commands;
using bnet.Responses; // for the timestamp converter extension. should probably move that..
using Discord.WebSocket;

namespace ilvlbot.Modules
{
	[Name("WoW Token")]
	public partial class WowToken : ModuleBase<SocketCommandContext>
	{
		private static readonly string wowEmoji = "wow".ToDiscordEmoji();
		private static readonly string goldEmoji = "gold".ToDiscordEmoji();
		private static readonly Color goldColor = new Color(217, 200, 107);

		private class Price
		{
			public string Symbol { get; }
			public decimal Amount { get; }

			public Price(string symbol, decimal amt)
			{
				Symbol = symbol;
				Amount = amt;
			}
		}

		private readonly Dictionary<string, Price> regionToTokenPrice = new Dictionary<string, Price>()
		{
			{ "NA", new Price("$", 20m) },
			{ "EU", new Price("€", 20m) },
			{ "GB", new Price("£", 20m) },
			{ "CN", new Price("¥", 75m) },
			{ "TW", new Price("NT$", 500m) },
			{ "KR", new Price("₩", 22000m) },
		};

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

			Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] [TOKEN] [Req:{chan}/{Context.Message.Author.Username}#{Context.Message.Author.Discriminator}]: {msg}{s}");
		}

		[Command("token"), Alias("wowtoken")]
		[Summary("Get the current price of a WoW token.")]
		[Remarks("Get the current price of a WoW token. Optional parameter, `region_name` can be any one of [`NA`, `EU`, `GB`, `CN`, `TW`, `KR`].")]
		public async Task GetTokenPrice([Remainder]string regionName = null)
		{
			if (string.IsNullOrEmpty(regionName))
			{
				regionName = Configuration.Settings.ItemLevelConfig.DefaultWowTokenRegion;
				Log($"No region specified in request, using default region: `{regionName}`");
			}
			
			try
			{
				Log($"Getting snapshot, reporting for region: `{regionName}`");

				var full_snapshot = await Api.GetSnapshot();

				if (full_snapshot == null)
				{
					throw new ArgumentNullException(nameof(full_snapshot));
				}

				Api.Region snapshot = full_snapshot.GetRegion(regionName);

				if (snapshot == null)
				{
					await ReplyAsync($"I can't find anything about the {regionName} region.");
					return;
				}

				decimal buy = snapshot.raw.buy;
				decimal last_min = snapshot.raw.twentyFourMin;
				decimal last_max = snapshot.raw.twentyFourMax;
				string updated = snapshot.formatted.updated;
				string region = snapshot.formatted.region;
				string tts = snapshot.formatted.timeToSell ?? "(an unknown amount of time)";

				var price = regionToTokenPrice[regionName];
				decimal gold_per_dollar = buy / price.Amount;

				// build the result
				string title = $"{wowEmoji} WoW Token: {goldEmoji}{buy:n0}";

				string body = $"As of {updated}:\n" +
						$"A token in the {region} region costs {goldEmoji}{buy:n0}.\n\n" +
						$"In the last 24 hours:\n" +
						$"The maximum value was {goldEmoji}{last_max:n0}.\n" +
						$"The minimum was {goldEmoji}{last_min:n0}.\n\n" +
						$"It currently takes {tts} to sell a WoW token.\n" +
						$"You'd make {gold_per_dollar:n2} gold per {price.Symbol}1 spent on the token.\n";

				var eb = new EmbedBuilder()
					.WithTitle(title)
					.WithColor(goldColor)
					.WithDescription(body)
					.WithFooter(x => x.WithText("via wowtoken.info"))
					.WithTimestamp(DateTime.SpecifyKind(snapshot.timestamp.UnixTimeToDateTime(), DateTimeKind.Utc))
					.Build();

				// print it to the channel
				await ReplyAsync("", embed: eb);
			}
			catch (Exception ex)
			{
				string output = $"I had a problem fetching the latest token price. Tell a nerd: {ex.Message} ({ex.GetCallingSite()})";
				Log(output, true);
				await ReplyAsync(output);
			}
		}
	}
}
