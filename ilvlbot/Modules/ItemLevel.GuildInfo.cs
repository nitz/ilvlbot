using bnet.Requests;
using bnet.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ilvlbot.Modules
{
	public partial class ItemLevel
	{
		private class GuildInfo
		{
			public int TargetLevel { get; private set; } = 110;
			public int MaxAsyncRequests { get; set; } = 35;

			public string Realm;
			public string Guild;

			public GuildMembers GuildResponse;
			public List<Member> GuildMembers;
			public List<string> FailedCharacters;
			public string LastError;

			public int CharacterCount { get; private set; }
			public int TargetLevelCharacterCount { get; private set; }

			public DateTime LastRefresh = DateTime.MinValue;

			public GuildInfo(string realm, string guild, int target_level)
			{
				Realm = realm;
				Guild = guild;
				TargetLevel = target_level;
			}

			internal static string MakeKey(string realm, string guild)
			{
				return $"!{realm}##{guild}!";
			}

			public string GetKey()
			{
				return MakeKey(Realm, Guild);
			}

			public async Task<bool> Refresh(Action<string> optional_output = null)
			{
				try
				{
					LastError = string.Empty;

					optional_output?.Invoke($"Getting info: \"{Realm}/{Guild}\"...");

					var rsp = await Get.GuildInfo(Realm, Guild);

					if (rsp.Successful == false)
					{
						if (rsp.StatusCode == System.Net.HttpStatusCode.NotFound)
							LastError = $"I couldn't find the guild `{Guild}` on `{Realm}`";
						else
							LastError = $"Something went wrong with the request. Tell a nerd: {rsp.ErrorText}";
						return false;
					}

					GuildResponse = rsp.Result;

					CharacterCount = GuildResponse.members.Count;

					var chars_at_110 = GuildResponse.members.Where(x => x.guildCharacter.level >= TargetLevel).ToList();

					TargetLevelCharacterCount = chars_at_110.Count;

					optional_output?.Invoke($"{CharacterCount} characters. {TargetLevelCharacterCount} are at level {TargetLevel}.");

					if (TargetLevelCharacterCount == 0)
						return true;

					int count = 0;
					var tasks = new List<Task>();

					// build tasks for each character to get more info for them.
					var fields = bnet.Requests.Fields.Character.Minimal;
					fields.Items = true;

					int active_requests = 0;

					foreach (var member in chars_at_110)
					{
						var fetch_task = Task.Run(async () =>
						{
							while (active_requests > MaxAsyncRequests)
								await Task.Delay(5);
							
							System.Threading.Interlocked.Increment(ref active_requests);

							try
							{
								await member.FetchSpecificCharacterInfo(fields);
							}
							finally
							{
								System.Threading.Interlocked.Decrement(ref active_requests);
							}

						});

						tasks.Add(fetch_task);

						++count;
					}

					await Task.WhenAll(tasks);

					if (active_requests > 0)
					{
						throw new InvalidOperationException($"Tasks finished but {nameof(active_requests)} still > 0?! ({active_requests})");
					}

					GuildMembers = chars_at_110.Where(x => x.character != null).OrderByDescending(x => x.character.items.calculatedItemLevel).ToList();
					FailedCharacters = chars_at_110.Where(x => x.character == null).OrderByDescending(x => x.guildCharacter.name).Select(x => x.guildCharacter.name).ToList();

					LastRefresh = DateTime.Now;

					optional_output?.Invoke($"Finished refreshing: \"{Realm}/{Guild}\"...");

					return true;
				}
				catch (Exception ex)
				{
					LastError = ex.Message;
					return false;
				}
			}
		}
	}
}
