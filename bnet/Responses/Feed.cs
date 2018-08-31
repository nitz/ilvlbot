using System.Collections.Generic;

namespace bnet.Responses
{
	public class Feed
	{
		public string type { get; set; }
		public long? timestamp { get; set; }
		public Achievement achievement { get; set; }
		public bool featOfStrength { get; set; }
		public int? itemId { get; set; }
		public string context { get; set; }
		public List<int> bonusLists { get; set; }
		public Criteria criteria { get; set; }
		public int? quantity { get; set; }
		public string name { get; set; }
	}
}
