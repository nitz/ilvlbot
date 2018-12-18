using bnet.Responses;
using bnet.Networking.Http;
using System.Threading.Tasks;
using System;

namespace bnet.Requests
{
	public class Get
	{
		public static async Task<RequestResult<GuildMembers>> GuildInfo(string realm, string guild_name)
		{
			string req_string = string.Format(Strings.guildMembersAndNewsApiFormat, realm, guild_name);
			return await Common.RequestAndDeserialize<GuildMembers>(req_string, CacheMode.Uncached, additionalHeaders: Api.Token);
		}

		public static async Task<RequestResult<Character>> CharacterInfo(string realm, string character, Fields.Character fields)
		{
			string req_string = string.Format(Strings.characterItemsApiFormat, realm, character, fields.ToQueryParam());
			return await Common.RequestAndDeserialize<Character>(req_string, CacheMode.Uncached, additionalHeaders: Api.Token);
		}

		public static async Task<RequestResult<CharacterClasses>> CharacterClasses()
		{
			string req_string = string.Format(Strings.characterClassesApiFormat);
			return await Common.RequestAndDeserialize<CharacterClasses>(req_string, CacheMode.Cached, additionalHeaders: Api.Token);
		}

		public static async Task<RequestResult<CharacterRaces>> CharacterRaces()
		{
			string req_string = string.Format(Strings.characterRacesApiFormat);
			return await Common.RequestAndDeserialize<CharacterRaces>(req_string, CacheMode.Cached, additionalHeaders: Api.Token);
		}

		public static async Task<RequestResult<CharacterAchievements>> Achievement(int id)
		{
			string req_string = string.Format(Strings.achievementApiFormat, id);
			return await Common.RequestAndDeserialize<CharacterAchievements>(req_string, CacheMode.Cached, additionalHeaders: Api.Token);
		}

		public static async Task<RequestResult<OAuthAccessToken>> AccessToken(Api.ClientSecret clientSecrets)
		{
			string req_string = string.Format(Strings.oAuthTokenFormat);
			var requestContent = new System.Net.Http.MultipartFormDataContent();
			requestContent.Add(new System.Net.Http.StringContent(Strings.oAuthTokenRequstValueGrantTypeClientCredentials), Strings.oAuthTokenRequstFieldGrantType);
			return await Common.RequestAndDeserialize<OAuthAccessToken>(req_string, CacheMode.Uncached, requestContent, additionalHeaders: clientSecrets);
		}
	}
}
