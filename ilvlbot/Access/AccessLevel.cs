
/// <summary>
/// Modified via https://github.com/Aux/Discord.Net-Example/blob/1.0/Discord.Net-Example/Common/Enums/AccessLevel.cs
/// </summary>
namespace ilvlbot.Access
{
	/// <summary>
	/// The enum used to specify permission levels. A lower
	/// number means less permissions than a higher number.
	/// </summary>
	public enum AccessLevel
	{
		Blocked,
		User,
		GuildModerator,
		GuildAdministrator,
		GuildOwner,
		BotOwner
	}
}
