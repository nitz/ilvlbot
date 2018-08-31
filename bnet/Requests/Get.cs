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
			string req_string = string.Format(Strings.guildMembersAndNewsApiFormat, Api.Keys, realm, guild_name);
			return await Common.RequestAndDeserialize<GuildMembers>(req_string);
		}

		public static async Task<RequestResult<Character>> CharacterInfo(string realm, string character, Fields.Character fields)
		{
			string req_string = string.Format(Strings.characterItemsApiFormat, Api.Keys, realm, character, fields.ToQueryParam());
			return await Common.RequestAndDeserialize<Character>(req_string);
		}

		public static async Task<RequestResult<CharacterClasses>> CharacterClasses()
		{
			string req_string = string.Format(Strings.characterClassesApiFormat, Api.Keys);
			return await Common.RequestAndDeserialize<CharacterClasses>(req_string, CacheMode.Cached);
		}

		public static async Task<RequestResult<CharacterRaces>> CharacterRaces()
		{
			string req_string = string.Format(Strings.characterRacesApiFormat, Api.Keys);
			return await Common.RequestAndDeserialize<CharacterRaces>(req_string, CacheMode.Cached);
		}

		public static async Task<RequestResult<CharacterAchievements>> Achievement(int id)
		{
			string req_string = string.Format(Strings.achievementApiFormat, Api.Keys, id);
			return await Common.RequestAndDeserialize<CharacterAchievements>(req_string, CacheMode.Cached);
		}
	}
}
