using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

			public static implicit operator KeyValuePair<string, string>(ClientSecret s)
			{
				string authString = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{s.ID}:{s.Secret}"));
				return new KeyValuePair<string, string>("Authorization", $"Basic {authString}");
			}
		}
	}
}
