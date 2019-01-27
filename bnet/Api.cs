using bnet.Requests;
using bnet.Responses;
using core.Services.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace bnet
{
	// todo -- actually see if someone has a fleshed out library so i'm not rolling my own ;)
	public static partial class Api
	{
		internal const string Tag = "bnet";

		private static IServiceProvider _services;
		private static ILogger _logger;

		public static ClientSecret Secrets { internal get; set; } = null;
		internal static OAuthAccessToken Token { get { lock (_accessToken) { return _accessToken; } } }

		private static OAuthAccessToken _accessToken = null;
		private static Task _accessTokenRenewal = null;
		private static CancellationTokenSource _accessTokenRenewalCancellation;
		private static TimeSpan _accessTokenRenewalFailRetry = TimeSpan.FromSeconds(3);
		private static TimeSpan _accessTokenRenewBefore = TimeSpan.FromSeconds(30);

		private static readonly TimeSpan FullDelay = TimeSpan.FromMilliseconds(int.MaxValue);

		public static bool Initialize(ClientSecret secrets, IServiceProvider services)
		{
			// store key, generate invalid access token.
			Secrets = secrets;
			_services = services;
			_accessToken = new OAuthAccessToken();

			// grab services
			_logger = _services.GetService<ILogger>();

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

					// no need to thrash the cpu while we wait.
					Task.Delay(1).Wait();
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

		/// <summary>
		/// Gets a slice of the OAuth token for administration display, as well as when it expires.
		/// </summary>
		/// <returns>A few characters of the OAuth token, plus the current expiry date.</returns>
		public static (string TokenSlice, DateTime CreatedAt, DateTime ExpiresAt) GetCurrentOAuthInfo()
		{
			string slice = _accessToken.AccessToken.Substring(0, 8) + "...";
			return (slice, _accessToken.CreatedAt, _accessToken.ExpiresAt);
		}

		public static async Task<bool> RenewOAuthTokenNowAsync()
		{
			var newToken = await Get.AccessToken(Secrets);

			if (newToken == null || newToken.Result == null)
			{
				// issue getting token
				return false;
			}

			if (newToken.Result.Valid == false)
			{
				// bad token!
				return false;
			}

			lock (_accessToken)
			{
				_accessToken = newToken;
			}

			return true;
		}

		private static void KeepAccessTokenRenewed()
		{
			while (_accessTokenRenewalCancellation.Token.IsCancellationRequested == false)
			{
				Log("Getting new OAuth access token...");

				bool gotNewToken = RenewOAuthTokenNowAsync().GetAwaiter().GetResult();

				if (gotNewToken == false)
				{
					Log($"Access token invalid, retrying in {_accessTokenRenewalFailRetry}...");
					Task.Delay(_accessTokenRenewalFailRetry, _accessTokenRenewalCancellation.Token).Wait();
				}

				var renewAt = _accessToken.ExpiresAt.Subtract(_accessTokenRenewBefore);
				var delayFor = renewAt - DateTime.Now;

				Log($"Successfully retrieved access token.");
				Log($"Will attempt to renew at {renewAt}, delaying for: {delayFor}");

				Task.Delay(delayFor, _accessTokenRenewalCancellation.Token).Wait();
			}
		}

		internal static void Log(string msg)
		{
			_logger.Log(Tag, msg);
		}
	}
}
