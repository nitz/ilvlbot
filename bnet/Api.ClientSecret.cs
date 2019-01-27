using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace bnet
{
	public partial class Api
	{
		public class ClientSecret
		{
			public string ID { get; set; }
			public string Secret { get; set; }

			public ClientSecret(string id, string secret)
			{
				ID = id;
				Secret = secret;
			}

			public static implicit operator AuthenticationHeaderValue(ClientSecret s)
			{
				string authString = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{s.ID}:{s.Secret}"));
				return new AuthenticationHeaderValue(AuthenticationSchemes.Basic.ToString(), authString);
			}
		}
	}
}
