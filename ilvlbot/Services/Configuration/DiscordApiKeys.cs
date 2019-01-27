
namespace ilvlbot.Services.Configuration
{
	/// <summary>
	/// A class representing a discord bot token, as well as a client ID
	/// </summary>
	public class DiscordApiKeys
	{
		public string ClientId { get; set; } = string.Empty;
		public string Token { get; set; } = string.Empty;

		public DiscordApiKeys(string client_id, string token)
		{
			ClientId = client_id;
			Token = token;
		}
	}
}
