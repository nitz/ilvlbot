using System;
using System.Collections.Generic;

namespace bnet.Responses
{
	public class Character : ApiResponse
	{
		public long lastModified { get; set; }
		public string name { get; set; }
		public string realm { get; set; }
		public string battlegroup { get; set; }
		public int @class { get; set; }
		public int race { get; set; }
		public int gender { get; set; }
		public int level { get; set; }
		public int achievementPoints { get; set; }
		public string thumbnail { get; set; }
		public string calcClass { get; set; }
		public int faction { get; set; }
		public int totalHonorableKills { get; set; }
		public List<Feed> feed { get; set; }
		public Items items { get; set; }
		public Stats stats { get; set; }
		public List<Title> titles { get; set; }
		public Achievements achievements { get; set; }
		public List<Talent> talents { get; set; }
		public CharacterAppearance appearance { get; set; }
		public Mounts mounts { get; set; }
		public Progression progression { get; set; }
		public Statistics statistics { get; set; }
		public Guild guild { get; set; }

		public DateTime lastModifiedDateTime
		{
			get
			{
				return lastModified.MsUnixTimeToDateTime();
			}
		}

		public string role
		{
			get
			{
				const string no_role = "???";

				if (talents == null)
					return no_role;

				foreach (var t in talents)
				{
					if (t.selected)
						return t.spec?.role ?? no_role;
				}

				return no_role;
			}
		}

		public string spec
		{
			get
			{
				const string no_spec = "Unspecialized";

				if (talents == null)
					return no_spec;

				foreach (var t in talents)
				{
					if (t.selected)
						return t.spec?.name ?? no_spec;
				}

				return no_spec;
			}
		}

		public string className
		{
			get
			{
				return @class.ToClass().name;
			}
		}

		public Class classInfo
		{
			get
			{
				return @class.ToClass();
			}
		}

		public string specClass
		{
			get { return $"{spec} {className}"; }
		}

		public string raceName
		{
			get
			{
				return race.ToRace().name;
			}
		}

		public string raceSide
		{
			get
			{
				return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(race.ToRace().side);
			}
		}

		public string avatarUrl
		{
			get
			{
				return Requests.Helpers.GenerateRenderUrl(thumbnail);
			}
		}

		public string profileImageUrl
		{
			get
			{
				// they only give me the avatar api, need to replace it with the profile image
				var frag = thumbnail.Replace(Requests.Strings.apiRenderAvatarSubstring, Requests.Strings.apiRenderProfileImageSubstring);
				return Requests.Helpers.GenerateRenderUrl(frag);
			}
		}

		public string insetImageUrl
		{
			get
			{
				// they only give me the avatar api, need to replace it with the inset image
				var frag = thumbnail.Replace(Requests.Strings.apiRenderAvatarSubstring, Requests.Strings.apiRenderInsetSubstring);
				return Requests.Helpers.GenerateRenderUrl(frag);
			}
		}

		public string armoryUrl
		{
			get
			{
				return Requests.Helpers.GenerateArmoryUrl(realm, name);
			}
		}
	}
}
