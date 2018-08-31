using System.Collections.Generic;

namespace bnet.Responses
{
	public class CharacterAppearance
	{
		public int faceVariation { get; set; }
		public int skinColor { get; set; }
		public int hairVariation { get; set; }
		public int hairColor { get; set; }
		public int featureVariation { get; set; }
		public bool showHelm { get; set; }
		public bool showCloak { get; set; }
		public List<int> customDisplayOptions { get; set; }
	}
}
