using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bnet
{
	public partial class Api
	{
		public class KeyPair
		{
			public string Key { get; set; }
			public string Secret { get; set; }

			public KeyPair(string key, string secret)
			{
				Key = key;
				Secret = secret;
			}

			public static implicit operator string(KeyPair k)
			{
				return k.ToString();
			}

			public override string ToString()
			{
				return Key;
			}
		}
	}
}
