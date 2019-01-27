using ilvlbot.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ilvlbot.Services.Configuration
{
	public class ItemLevelBotConfig
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
		public int[] CheckedAchievements { get; set; } = { AotcJaina, AotcUunat };

		/// <summary>
		/// The default region to use for WoW Token Price Checking
		/// </summary>
		public static string DefaultWowTokenRegion { get; internal set; } = Modules.WowToken.Api.Region.NA;
	}
}
