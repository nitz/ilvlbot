using System.Collections.Generic;

namespace bnet.Responses
{
	public class GuildMembers : ApiResponse
	{
		public long lastModified { get; set; }
		public string name { get; set; }
		public string realm { get; set; }
		public string battlegroup { get; set; }
		public int level { get; set; }
		public int side { get; set; }
		public int achievementPoints { get; set; }
		public List<Member> members { get; set; }
		public Emblem emblem { get; set; }
		public List<News> news { get; set; }
	}
}
