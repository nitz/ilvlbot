using System;
using ilvlbot.Extensions;

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
			public static ParsedCommand Parse(string remainder, char args_splitter, string default_realm, string default_target)
			{
				if (remainder.Length == 0)
				{
					// they gave me no command, just use the defaults.
					return new ParsedCommand()
					{
						RealmName = default_realm,
						TargetName = default_target,
					};
				}

				// there's at least a bit of an argument
				string[] splits = remainder.Split(new char[] { args_splitter }, 2);

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
						RealmName = default_realm,
						TargetName = splits[0],
					};
				}
			}
		}
	}
}
