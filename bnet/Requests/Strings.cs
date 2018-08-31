using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bnet.Requests
{
	internal class Strings
	{
		internal const string apiKeyField = "apikey";

		internal const string apiProtocol = "https://";
		internal const string apiRegion = "us"; // would need to change for regions.
		internal const string battleNetUri = "battle.net";
		internal const string apiWarcraftUri = "worldofwarcraft.com";
		internal const string apiUri = apiProtocol + apiRegion + ".api." + battleNetUri;
		internal const string apiLocale = "en_US"; // this also would likely vary.
		internal const string apiLocaleShort = "en";
		internal const string apiKeyFormat = "&" + apiKeyField + "={0}";
		internal const string guildMembersAndNewsApiFormat = apiUri + "/wow/guild/{1}/{2}?fields=members,news&locale=" + apiLocale + apiKeyFormat;
		internal const string guildNewsApiFormat = apiUri + "/wow/guild/{1}/{2}?fields=news&locale=" + apiLocale + apiKeyFormat;
		internal const string characterItemsApiFormat = apiUri + "/wow/character/{1}/{2}?fields={3}&locale=" + apiLocale + apiKeyFormat;
		internal const string characterClassesApiFormat = apiUri + "/wow/data/character/classes?locale=" + apiLocale + apiKeyFormat;
		internal const string characterRacesApiFormat = apiUri + "/wow/data/character/races?locale=" + apiLocale + apiKeyFormat;
		internal const string achievementApiFormat = apiUri + "/wow/achievement/{1}?locale=" + apiLocale + apiKeyFormat;

		// the profile image renderer doesn't use the battle.net, and instead uses worldofwarcraft.com
		// see the migration guide here: https://us.battle.net/forums/en/bnet/topic/20752136188
		internal const string apiRenderSubdomain = "render-";
		internal const string apiRenderProfileImageSubstring = "-main.jpg";
		internal const string apiRenderAvatarSubstring = "-avatar.jpg";
		internal const string apiRenderInsetSubstring = "-inset.jpg";

		internal const string apiRenderBaseUri = apiProtocol + apiRenderSubdomain + apiRegion + "." + apiWarcraftUri + "/character/";
		internal const string armoryProfileBaseUri = apiProtocol + apiWarcraftUri + "/" + apiLocaleShort + "-" + apiRegion + "/character/";
	}
}
