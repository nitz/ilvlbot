
using Newtonsoft.Json;
using System;
using System.Net.Http.Headers;

namespace bnet.Responses
{
	public partial class OAuthAccessToken
	{
		[JsonProperty("access_token")]
		public string AccessToken { get; set; }

		[JsonProperty("token_type")]
		public string TokenType { get; set; }

		[JsonProperty("expires_in")]
		private long _expiresInSeconds { get; set; }
		
		[JsonProperty("scope")]
		public string Scope { get; set; }
		
		[JsonIgnore]
		public DateTime ExpiresAt => CreatedAt.AddSeconds(_expiresInSeconds);

		[JsonIgnore]
		public DateTime CreatedAt { get; private set; }

		[JsonIgnore]
		public bool Valid => string.IsNullOrEmpty(AccessToken) == false && ExpiresAt > DateTime.Now;
		
		public OAuthAccessToken()
		{
			CreatedAt = DateTime.Now;
		}

		public static implicit operator AuthenticationHeaderValue(OAuthAccessToken t)
		{
			return new AuthenticationHeaderValue("Bearer", t.AccessToken);
		}
 }
}
