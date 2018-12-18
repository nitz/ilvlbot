
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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
		
		private DateTime _creationTimeStamp;
		
		[JsonIgnore]
		public DateTime ExpiresAt => _creationTimeStamp.AddSeconds(_expiresInSeconds);

		[JsonIgnore]
		public bool Valid => string.IsNullOrEmpty(AccessToken) == false && ExpiresAt > DateTime.Now;
		
		public OAuthAccessToken()
		{
			_creationTimeStamp = DateTime.Now;
		}

		public static implicit operator KeyValuePair<string, string>(OAuthAccessToken t)
		{
			return new KeyValuePair<string, string>("Authorization", $"Bearer {t.AccessToken}");
		}
 }
}
