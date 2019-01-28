using System;

namespace ilvlbot.Extensions
{
	/// <summary>
	/// An extension class for a few string operations used throught the commands.
	/// </summary>
	public static class StringExtensions
	{
		public static string ToRealmUri(this string s)
		{
			return s.Replace(" ", "-");
		}

		private static Random rand = new Random();
		public static string GetRandom(this string[] arr)
		{
			return arr[rand.Next(arr.Length)];
		}
	}
}
