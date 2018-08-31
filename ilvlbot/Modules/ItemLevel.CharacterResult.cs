using Discord;

namespace ilvlbot.Modules
{
	public partial class ItemLevel
	{
		/// <summary>
		/// A small class to handle the result returned from the Character function.
		/// </summary>
		private class CharacterResult
		{
			public Embed EmbedResult { get; set; }
			public string StringResult { get; set; }

			public bool IsEmbedResult { get { return EmbedResult != null; } }
			public bool IsStringResult { get { return StringResult != null; } }

			public CharacterResult(string res)
			{
				StringResult = res;
				EmbedResult = null;
			}

			public CharacterResult(Embed res)
			{
				StringResult = null;
				EmbedResult = res;
			}
		}
	}
}
