using System.Collections.Generic;

namespace bnet.Responses
{
	public class News
	{
		public string type { get; set; }
		public string character { get; set; }
		public object timestamp { get; set; }
		public int itemId { get; set; }
		public string context { get; set; }
		public List<int> bonusLists { get; set; }
		public Achievement achievement { get; set; }
	}
}
