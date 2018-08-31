using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ilvlbot.Extensions
{
	public static class DiscordEmojiExtensions
	{
		public static string ToDiscordEmoji(this string s)
		{
			if (Program.Settings.Emoji.ContainsKey(s))
				return Program.Settings.Emoji[s];

			return string.Empty;
		}
	}
}
