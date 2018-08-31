using System.Collections.Generic;

namespace bnet.Responses
{
	public class Achievements
	{
		public List<int> achievementsCompleted { get; set; }
		public List<ulong> achievementsCompletedTimestamp { get; set; }
		public List<ulong> criteria { get; set; }
		public List<ulong> criteriaQuantity { get; set; }
		public List<ulong> criteriaTimestamp { get; set; }
		public List<ulong> criteriaCreated { get; set; }
	}
}
