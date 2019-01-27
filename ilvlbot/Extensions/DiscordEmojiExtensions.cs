using ilvlbot.Services.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ilvlbot.Extensions
{
	public static class DiscordEmojiExtensions
	{
		private static Settings _settings = null;

		public static string ToDiscordEmoji(this string s)
		{
			if (_settings == null)
			{
				_settings = Program.Services.GetService<Settings>();
			}

			if (_settings.Emoji.ContainsKey(s))
			{
				return _settings.Emoji[s];
			}

			return string.Empty;
		}
	}
}
