using System.Diagnostics;

namespace bnet.Responses
{
	[DebuggerDisplay("weaponSpeed = {weaponSpeed}, dps = {dps}")]
	public class WeaponInfo
	{
		public Damage damage { get; set; }
		public double weaponSpeed { get; set; }
		public double dps { get; set; }
	}
}
