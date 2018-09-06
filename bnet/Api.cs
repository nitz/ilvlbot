using bnet.Responses;
using bnet.Requests;
using bnet.Networking.Http;

namespace bnet
{
	// todo -- actually see if someone has a fleshed out library so i'm not rolling my own ;)
	public static partial class Api
	{
		public static KeyPair Keys { internal get; set; } = null;

		public static bool Initialize(KeyPair apiKeys)
		{
			// store key,
			Keys = apiKeys;

			// set up network to not print our apikey!
			Common.AddForbiddenString(Strings.apiKeyField);

			try
			{
				// set up the extension methods with the data they need.
				Extensions.GetStaticInformation();
				return true;
			}
			catch (System.Exception /*ex*/)
			{
				// failed to get static info for some  reason... we should probably handle that somehow.
				// for now, let our caller handle the failure, probably by retrying.
				return false;
			}
		}
	}
}
