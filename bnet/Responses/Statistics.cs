using System.Collections.Generic;

namespace bnet.Responses
{
	public class Statistics
	{
		public int id { get; set; }
		public string name { get; set; }
		public List<SubCategory> subCategories { get; set; }
	}
}
