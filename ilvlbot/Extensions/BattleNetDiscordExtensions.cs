
namespace ilvlbot.Extensions
{
	public static class BattleNetDiscordExtensions
	{
		public static Discord.Color ToDiscordColor(this bnet.ClassColor c)
		{
			return new Discord.Color(c.Red, c.Green, c.Blue);
		}
	}
}
