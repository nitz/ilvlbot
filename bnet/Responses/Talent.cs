using System.Collections.Generic;

namespace bnet.Responses
{
	public class Talent
	{
		public bool selected { get; set; }
		public List<object> talents { get; set; }
		public Spec spec { get; set; }
		public string calcTalent { get; set; }
		public string calcSpec { get; set; }
	}
}
