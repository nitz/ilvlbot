using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bnet.Requests
{
	internal class Strings
	{
		[Obsolete("No longer using query string keys.")]
		internal const string apiKeyField = "--unused--";

		internal const string apiProtocol = "https://";
		internal const string apiRegion = "us"; // would need to change for regions.
		internal const string battleNetUri = "battle.net";
		internal const string blizzardComUri = "blizzard.com";
		internal const string apiWarcraftUri = "worldofwarcraft.com";
		internal const string apiUri = apiProtocol + apiRegion + ".api." + blizzardComUri;
		internal const string apiLocale = "en_US"; // this also would likely vary.
		internal const string apiLocaleShort = "en";

		[Obsolete("No longer using query string keys.")]
		internal const string apiKeyFormat = "--unused--";
		internal const string guildMembersAndNewsApiFormat = apiUri + "/wow/guild/{0}/{1}?fields=members,news&locale=" + apiLocale;
		internal const string guildNewsApiFormat = apiUri + "/wow/guild/{0}/{1}?fields=news&locale=" + apiLocale;
		internal const string characterItemsApiFormat = apiUri + "/wow/character/{0}/{1}?fields={2}&locale=" + apiLocale;
		internal const string characterClassesApiFormat = apiUri + "/wow/data/character/classes?locale=" + apiLocale;
		internal const string characterRacesApiFormat = apiUri + "/wow/data/character/races?locale=" + apiLocale;
		internal const string achievementApiFormat = apiUri + "/wow/achievement/{0}?locale=" + apiLocale;

		// the profile image renderer doesn't use the battle.net, and instead uses worldofwarcraft.com
		// see the migration guide here: https://us.battle.net/forums/en/bnet/topic/20752136188
		internal const string apiRenderSubdomain = "render-";
		internal const string apiRenderProfileImageSubstring = "-main.jpg";
		internal const string apiRenderAvatarSubstring = "-avatar.jpg";
		internal const string apiRenderInsetSubstring = "-inset.jpg";

		internal const string apiRenderBaseUri = apiProtocol + apiRenderSubdomain + apiRegion + "." + apiWarcraftUri + "/character/";
		internal const string armoryProfileBaseUri = apiProtocol + apiWarcraftUri + "/" + apiLocaleShort + "-" + apiRegion + "/character/";

		// OAuth related, apparently they want to use battlenet uri rather than blizzard.com?
		internal const string oAuthBaseUri = apiProtocol + apiRegion + "." + battleNetUri + "/oauth";
		internal const string oAuthTokenFormat = oAuthBaseUri + "/token";
		internal const string oAuthTokenRequstFieldGrantType = "grant_type";
		internal const string oAuthTokenRequstValueGrantTypeClientCredentials = "client_credentials";
	}
}
