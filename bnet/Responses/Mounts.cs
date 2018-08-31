using System.Collections.Generic;

namespace bnet.Responses
{
	public class Mounts
	{
		public int numCollected { get; set; }
		public int numNotCollected { get; set; }
		public List<Mount> collected { get; set; }
	}
}
