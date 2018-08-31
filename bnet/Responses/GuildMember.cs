
namespace bnet.Responses
{
	public class GuildMember
	{
		public string name { get; set; }
		public string realm { get; set; }
		public string battlegroup { get; set; }
		public int @class { get; set; }
		public int race { get; set; }
		public int gender { get; set; }
		public int level { get; set; }
		public int achievementPoints { get; set; }
		public string thumbnail { get; set; }
		public string guild { get; set; }
		public string guildRealm { get; set; }
		public int lastModified { get; set; }
		public Spec spec { get; set; }
	}
}
