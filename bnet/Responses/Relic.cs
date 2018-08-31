using System.Collections.Generic;
using System.Diagnostics;

namespace bnet.Responses
{
	[DebuggerDisplay("socket = {socket}, itemId = {itemId}")]
	public class Relic
	{
		public int socket { get; set; }
		public int itemId { get; set; }
		public int context { get; set; }
		public List<int> bonusLists { get; set; }
	}
}
