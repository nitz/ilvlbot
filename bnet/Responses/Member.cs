using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace bnet.Responses
{
	public class Member
	{
		// ignore 'character', this is one I populate with the 'Fetch' functions.
		// this is the 'full' character info that won't be populated by the guild api.
		[JsonIgnore]
		public Character character { get; private set; } = null;

		public bool hasCharacter { get { return character != null; } }

		// this is the 'character' returned in the bnet api. It isn't a full Character,
		// it's more of a 'lite' Character.
		[JsonProperty("character")]
		public GuildMember guildCharacter { get; set; }
		public int rank { get; set; }

		public async Task<bool> FetchBasicCharacterInfo()
		{
			return await FetchSpecificCharacterInfo(Requests.Fields.Character.Minimal);
		}

		public async Task<bool> FetchSlimCharacterInfo()
		{
			return await FetchSpecificCharacterInfo(Requests.Fields.Character.Slim);
		}

		public async Task<bool> FetchFullCharacterInfo()
		{
			return await FetchSpecificCharacterInfo(Requests.Fields.Character.Full);
		}

		public async Task<bool> FetchSpecificCharacterInfo(Requests.Fields.Character fields)
		{
			if (character == null)
			{
				try
				{
					// attempt to populate!
					character = await Requests.Get.CharacterInfo(guildCharacter.realm, guildCharacter.name, fields);
				}
				catch (Exception)
				{
					// failed to get character information!
					character = null;
					return false;
				}
			}

			return true;
		}
	}
}
