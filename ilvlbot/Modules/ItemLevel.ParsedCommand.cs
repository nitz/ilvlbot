using ilvlbot.Extensions;
using System;

namespace ilvlbot.Modules
{
	public partial class ItemLevel
	{
		/// <summary>
		/// A class for accessing the bits and peices of a command.
		/// </summary>
		private class ParsedCommand
		{
			public string RealmName { get; set; }
			public string TargetName { get; set; }

			private ParsedCommand()
			{

			}

			// this whole class could probably be replaced with a nice regex.
			public static ParsedCommand Parse(string remainder, string defaultRealm, string defaultTarget)
			{
				if (remainder.Length == 0)
				{
					// they gave me no command, just use the defaults.
					return new ParsedCommand()
					{
						RealmName = defaultRealm,
						TargetName = defaultTarget,
					};
				}

				// there's at least a bit of an argument
				string[] splits = SplitsByPotentialSyntax(remainder);

				if (splits.Length == 2)
				{
					// they gave me a realm and a target.
					return new ParsedCommand()
					{
						RealmName = splits[0].ToRealmUri(),
						TargetName = splits[1]
					};
				}
				else
				{
					// they only gave me a target, assume default realm.
					return new ParsedCommand()
					{
						RealmName = defaultRealm,
						TargetName = splits[0],
					};
				}
			}

			private static string[] SplitsByPotentialSyntax(string val)
			{
				if (val.Contains("/"))
				{
					// eg "My Realm/My Guild"
					// traditional full syntax, split on the slash.
					return val.Split(new char[] { '/' }, 2);
				}
				else if (val.StartsWith("\"") && val.EndsWith("\"") && val.Contains("-"))
				{
					// eg: "My Guild-My Realm"
					// alternative, full-quoted syntax. remove quotes and split by hyphen.
					// then flip the array because the realm comes second in this syntax.
					var res = val.Trim(new[] { '"' }).Split(new[] { '-' }, 2);
					Array.Reverse(res);
					return res;
				}

				// no slash or dash, assume it's just a guild/character name,
				// allow the default to be used for realm.
				return new[] { val };
			}
		}
	}
}
