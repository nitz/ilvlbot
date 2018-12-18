using bnet.Requests;
using bnet.Responses;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace bnet
{
	// todo -- actually see if someone has a fleshed out library so i'm not rolling my own ;)
	public static partial class Api
	{
		public static ClientSecret Secrets { internal get; set; } = null;
		internal static System.Action<string> Log { get; set; } = (s) => { };
		internal static KeyValuePair<string, string> Token { get { lock (_accessToken) { return _accessToken; } } }

		private static OAuthAccessToken _accessToken = null;
		private static Task _accessTokenRenewal = null;
		private static CancellationTokenSource _accessTokenRenewalCancellation;
		private static TimeSpan _accessTokenRenewalFailRetry = TimeSpan.FromSeconds(3);
		private static TimeSpan _accessTokenRenewBefore = TimeSpan.FromSeconds(30);

		private static readonly TimeSpan FullDelay = TimeSpan.FromMilliseconds(int.MaxValue);

		public static bool Initialize(ClientSecret secrets, System.Action<string> log)
		{
			// store key, generate invalid access token.
			Secrets = secrets;
			Log = log;
			_accessToken = new OAuthAccessToken();

			try
			{
				// set up timer to refresh access token.
				// this is probably not the best way to do this.
				_accessTokenRenewalCancellation = new CancellationTokenSource();
				_accessTokenRenewal = Task.Factory.StartNew(KeepAccessTokenRenewed, _accessTokenRenewalCancellation.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

				DateTime failAt = DateTime.Now.AddSeconds(5);

				while (true)
				{
					lock (_accessToken)
					{
						if (_accessToken.Valid)
						{
							// good to start!
							break;
						}
					}

					if (DateTime.Now >= failAt)
					{
						Log("Failed to get initial access token.");

						_accessTokenRenewalCancellation.Cancel(throwOnFirstException: true);

						return false;
					}
				}

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

		private static void KeepAccessTokenRenewed()
		{
			while (_accessTokenRenewalCancellation.Token.IsCancellationRequested == false)
			{
				Log("Getting new OAuth access token...");
				var newToken = Get.AccessToken(Secrets).GetAwaiter().GetResult();

				if (newToken.Result.Valid == false)
				{
					// bad token! wait and try again.
					Log($"Access token invalid, retrying in {_accessTokenRenewalFailRetry}...");
					Task.Delay(_accessTokenRenewalFailRetry, _accessTokenRenewalCancellation.Token).Wait();
					continue;
				}

				lock (_accessToken)
				{
					_accessToken = newToken;
				}

				var renewAt = _accessToken.ExpiresAt.Subtract(_accessTokenRenewBefore);
				var delayFor = renewAt - DateTime.Now;

				Log($"Successfully retrieved access token.");
				Log($"Will attempt to renew at {renewAt}, delaying for: {delayFor}");

				Task.Delay(delayFor, _accessTokenRenewalCancellation.Token).Wait();
			}
		}
	}
}
