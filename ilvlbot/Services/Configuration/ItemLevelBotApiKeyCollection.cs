using System.Linq;

namespace ilvlbot.Services.Configuration
{
	/// <summary>
	/// A collection of all the API keys used by ilvlbot.
	/// </summary>
	public class ItemLevelBotApiKeyCollection
	{
		public DiscordApiKeys Discord { get; internal set; } = new DiscordApiKeys("", "");
		public bnet.Api.ClientSecret BattleNet { get; internal set; } = new bnet.Api.ClientSecret("", "");

		[Newtonsoft.Json.JsonIgnore]
		public bool KeysSet
		{
			get
			{
				return new[] { Discord.ClientId, Discord.Token, BattleNet.ID, BattleNet.Secret }.All(x => string.IsNullOrEmpty(x) == false);
			}
		}
	}
}
