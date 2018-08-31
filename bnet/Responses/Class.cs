
namespace bnet.Responses
{
	public class Class
	{
		public int id { get; set; }
		public int mask { get; set; }
		public string powerType { get; set; }
		public string name { get; set; }

		// helper to convert the character's id to a class color
		public ClassColor color { get { return id; } }
	}
}
