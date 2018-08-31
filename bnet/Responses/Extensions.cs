using System;

namespace bnet.Responses
{
	// #todo -- these can likely be broken up into logical files instead of clumped all in this one.
	public static class Extensions
	{
		private static readonly DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public static DateTime MsUnixTimeToDateTime(this long time_since_epoch_in_ms)
		{
			return unixEpoch.AddMilliseconds(time_since_epoch_in_ms);
		}

		public static DateTime UnixTimeToDateTime(this int time_since_epoch_in_seconds)
		{
			return unixEpoch.AddSeconds(time_since_epoch_in_seconds);
		}

		public static CharacterClasses Classes { get; private set; }
		public static CharacterRaces Races { get; private set; }

		internal static void GetStaticInformation()
		{
			var cl_task = Requests.Get.CharacterClasses();
			var ra_task = Requests.Get.CharacterRaces();

			Classes = cl_task.GetAwaiter().GetResult();
			Races = ra_task.GetAwaiter().GetResult();
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
