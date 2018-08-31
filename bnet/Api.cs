using bnet.Responses;
using bnet.Requests;
using bnet.Networking.Http;

namespace bnet
{
	// todo -- actually see if someone has a fleshed out library so i'm not rolling my own ;)
	public static partial class Api
	{
		public static KeyPair Keys { internal get; set; } = null;

		public static void Initialize(KeyPair apiKeys)
		{
			// store key,
			Keys = apiKeys;

			// set up network to not print our apikey!
			Common.AddForbiddenString(Strings.apiKeyField);

			try
			{
				// set up the extension methods with the data they need.
				Extensions.GetStaticInformation();
			}
			catch (System.Net.WebException wex)
			{
				// if it's a 403, just re-throw.
				if ((wex.Response as System.Net.HttpWebResponse)?.StatusCode == System.Net.HttpStatusCode.Forbidden)
					throw wex;

				// failed to get static info for some other reason... we should probably handle that somehow.
			}
		}
	}
}
