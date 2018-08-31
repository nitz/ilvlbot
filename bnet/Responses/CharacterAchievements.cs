using System.Collections.Generic;

namespace bnet.Responses
{
	public class CharacterAchievements : ApiResponse
	{
		public int id { get; set; }
		public string title { get; set; }
		public int points { get; set; }
		public string description { get; set; }
		public string reward { get; set; }
		public List<RewardItem> rewardItems { get; set; }
		public string icon { get; set; }
		public List<Criterion> criteria { get; set; }
		public bool accountWide { get; set; }
		public int factionId { get; set; }
	}
}
