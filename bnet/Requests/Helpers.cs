
namespace bnet.Requests
{
	public static class Helpers
	{
		internal static string GenerateRenderUrl(string thumbnail_path)
		{
			return Strings.apiRenderBaseUri + thumbnail_path;
		}

		internal static string GenerateArmoryUrl(string realm, string name)
		{
			return Strings.armoryProfileBaseUri + realm.Replace(' ', '-') + "/" + name;
		}
	}
}
