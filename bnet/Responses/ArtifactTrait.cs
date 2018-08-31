using System.Diagnostics;

namespace bnet.Responses
{
	[DebuggerDisplay("id = {id}, rank = {rank}")]
	public class ArtifactTrait
	{
		public int id { get; set; }
		public int rank { get; set; }
	}
}
