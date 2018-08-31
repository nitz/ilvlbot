using System.Diagnostics;

namespace bnet.Responses
{
	[DebuggerDisplay("exactMin = {exactMin}, exactMax = {exactMax}")]
	public class Damage
	{
		public double min { get; set; }
		public double max { get; set; }
		public double exactMin { get; set; }
		public double exactMax { get; set; }
	}
}
