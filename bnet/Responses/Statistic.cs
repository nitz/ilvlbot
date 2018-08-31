
namespace bnet.Responses
{
	public class Statistic
	{
		public int id { get; set; }
		public string name { get; set; }
		public ulong quantity { get; set; }
		public ulong lastUpdated { get; set; }
		public bool money { get; set; }
		public string highest { get; set; }
	}
}
