using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace ilvlbot.Services.Configuration
{
	public class Settings
	{
		public string Prefix { get; set; } = "!";
		public ItemLevelBotConfig ItemLevel { get; } = new ItemLevelBotConfig();
		public ItemLevelBotApiKeyCollection ApiKeys { get; } = new ItemLevelBotApiKeyCollection();
		
		public Dictionary<string, string> Emoji = new Dictionary<string, string>()
		{
			// general warcraft
			{ "wow", "<:wow:280479920738533376>" },
			{ "gold", "<:gold:280485596420505600>" },

			// factions
			{ "Alliance", "<:alliance:280376177561174018>" },
			{ "Horde", "<:horde:280376173488504832>" },

			// classes
			{ "Death Knight", "<:death_knight:280198284163809280>" },
			{ "Demon Hunter", "<:demon_hunter:280198283463360512>" },
			{ "Druid", "<:druid:280198283606097920>" },
			{ "Hunter", "<:hunter:280198283912282122>" },
			{ "Mage", "<:mage:280198283576475648>" },
			{ "Monk", "<:monk:280198283639521280>" },
			{ "Paladin", "<:paladin:280198283631001600>" },
			{ "Priest", "<:priest:280198283723407360>" },
			{ "Rogue", "<:rogue:280198283371216897>" },
			{ "Shaman", "<:shaman:280198283652104193>" },
			{ "Warlock", "<:warlock:280198284650217472>" },
			{ "Warrior", "<:warrior:280198283903893504>" },

			// other
			{ "ggstare", "<:ggstare:230180638882267136>" }
		};

		public ulong[] Owners = { 116026533688115204 };

		public Settings(string file)
		{
			Load(file);
		}

		private bool Load(string file)
		{
			try
			{
				var text = File.ReadAllText(file);

				var settings = new JsonSerializerSettings()
				{
					NullValueHandling = NullValueHandling.Ignore,
					MissingMemberHandling = MissingMemberHandling.Ignore
				};

				JsonConvert.PopulateObject(text, this, settings);
			}
			catch (Exception ex)
			{
				// #todo -- handle this nicer.
				Console.WriteLine($"Couldn't read settings: {ex.Message}");
				File.WriteAllText(file, JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented));

				Console.WriteLine($"Wrote an empty file to '{file}', fill it out and run again!");
				return false;
			}

			if (ApiKeys.KeysSet == false)
			{
				Console.WriteLine("API ID/Secret/Keys not all set.\n" +
					"Please check your settings.conf and ensure you've set everything.\n" +
					"As well, please note after moving to the Blizzard OAuth API, the BattleNet\n" +
					"Now expexts 'ID' and 'Secret' rather than 'Key' and 'Secret'.\n" +
					"Get your ID/Secret at https://develop.battle.net/access/clients");
				return false;
			}

			return true;
		}
	}
}
