using System;
using System.Threading.Tasks;

namespace bnet.Responses
{
	// #todo -- these can likely be broken up into logical files instead of clumped all in this one.
	public static class Extensions
	{
		private static readonly DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public static DateTime MsUnixTimeToDateTime(this long timeSinceEpochInMilliseconds)
		{
			return unixEpoch.AddMilliseconds(timeSinceEpochInMilliseconds);
		}

		public static DateTime UnixTimeToDateTime(this int timeSinceEpochInSeconds)
		{
			return unixEpoch.AddSeconds(timeSinceEpochInSeconds);
		}

		public static CharacterClasses Classes { get; private set; }
		public static CharacterRaces Races { get; private set; }

		internal static async Task GetStaticInformationAsync()
		{
			Classes = await Requests.Get.CharacterClasses();
			Races = await Requests.Get.CharacterRaces();
		}

		public static Class ToClass(this int cl)
		{
			foreach (var c in Classes.classes)
			{
				if (c.id == cl)
					return c;
			}

			return new Class() { id = cl, mask = 0, powerType = "power", name = "Unknown" };
		}

		public static Race ToRace(this int ra)
		{
			foreach (var r in Races.races)
			{
				if (r.id == ra)
					return r;
			}

			return new Race() { id = ra, mask = 0, side = "Unknown", name = "Unknown" };
		}

	}
}
