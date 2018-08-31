
namespace bnet.Responses
{
	public class Boss
	{
		public long id { get; set; }
		public string name { get; set; }
		public long? normalKills { get; set; }
		public long? normalTimestamp { get; set; }
		public long? heroicKills { get; set; }
		public long? heroicTimestamp { get; set; }
		public long? lfrKills { get; set; }
		public long? lfrTimestamp { get; set; }
		public long? mythicKills { get; set; }
		public long? mythicTimestamp { get; set; }
	}
}
