using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ilvlbot.Extensions;

namespace ilvlbot.Configuration
{
	public class Settings
	{
		public string Prefix { get; set; } = "!";
		public ItemLevelConfig ItemLevel { get; } = new ItemLevelConfig();
		public ApiKeyConfig ApiKeys { get; } = new ApiKeyConfig();
		
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

		private Settings()
		{

		}

		public class ApiKeyConfig
		{
			public DiscordKeys Discord { get; internal set; } = new DiscordKeys("", "", "");
			public bnet.Api.ClientSecret BattleNet { get; internal set; } = new bnet.Api.ClientSecret("", "");
			[Newtonsoft.Json.JsonIgnore]
			public bool KeysSet { get { return new[] { Discord.Token, Discord.ClientId, Discord.ClientSecret, BattleNet.ID, BattleNet.Secret }.All(x => string.IsNullOrEmpty(x)); } }
		}

		public class DiscordKeys
		{
			public string ClientId { get; set; } = "unset";
			public string ClientSecret { get; set; } = "unset";
			public string Token { get; set; } = "unset";

			public DiscordKeys(string client_id, string client_secret, string token)
			{
				ClientId = client_id;
				ClientSecret = client_secret;
				Token = token;
			}
		}

		public class ItemLevelConfig
		{
			private string _defaultRealm = "Stormrage";
			
			public TimeSpan GuildRequestCooldown { get; set; } = TimeSpan.FromMinutes(3);
			public int MaxGuildCharacters { get; set; } = 30;
			public string DefaultRealm { get { return _defaultRealm.ToRealmUri(); } set { _defaultRealm = value; } }
			public string DefaultGuild { get; set; } = "Gently Wafting Curtains";
			public int GuildTargetLevel { get; set; } = 120;

			// Some achievement IDs that might want to be checked.
			public const int AotcXavius = 11194;
			public const int AotcHelya = 11581;
			public const int AotcGuldan = 11195;
			public const int AotcKilJaeden = 11874;
			public const int AotcGhuun = 12536;
			public const int AotcJaina = 13322;
			public const int AotcUunat = 13418;

			/// <summary>
			/// A list of achievement ids to check when an individual character is looked up.
			/// </summary>
			public int[] CheckedAchievements { get; set; } = { AotcUunat, AotcJaina, AotcGhuun };

			/// <summary>
			/// The default region to use for WoW Token Price Checking
			/// </summary>
			public static string DefaultWowTokenRegion { get; internal set; } = Modules.WowToken.Api.Region.NA;
		}

		public static Settings Load(string file)
		{
			var settings =  new Settings();

			try
			{
				string text = File.ReadAllText(file);
				settings = Newtonsoft.Json.JsonConvert.DeserializeObject<Settings>(text);
			}
			catch (Exception ex)
			{
				// #todo -- handle this nicer.
				Console.WriteLine($"Couldn't read settings: {ex.Message}");

				File.WriteAllText(file, Newtonsoft.Json.JsonConvert.SerializeObject(new Settings(), Newtonsoft.Json.Formatting.Indented));

				Console.WriteLine($"Wrote an empty file to '{file}', fill it out and run again!\nPress any key to exit.");
				Console.ReadKey();
				Environment.Exit(-1);
			}

			return settings;
		}
	}
}
