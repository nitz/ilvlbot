using System.Collections.Generic;

namespace bnet.Responses
{
	public class SubCategory
	{
		public int id { get; set; }
		public string name { get; set; }
		public List<Statistic> statistics { get; set; }
		public List<SubCategory> subCategories { get; set; }
	}
}
