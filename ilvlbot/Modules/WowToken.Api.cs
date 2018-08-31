using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ilvlbot.Extensions;

namespace ilvlbot.Modules
{
	public partial class WowToken
	{
		public static class Api
		{
			private const string apiUri = "https://data.wowtoken.info/snapshot.json";

			#region Response

			public class Raw
			{
				public int buy { get; set; }
				[JsonProperty("24min")]
				public int twentyFourMin { get; set; }
				[JsonProperty("24max")]
				public int twentyFourMax { get; set; }
				public int? timeToSell { get; set; }
				public object timeToSellSeconds { get; set; }
				public int result { get; set; }
				public int updated { get; set; }
				public string updatedISO8601 { get; set; }
			}

			public class Formatted
			{
				public string buy { get; set; }
				[JsonProperty("24min")]
				public string twentyFourMin { get; set; }
				[JsonProperty("24max")]
				public string twentyFourMax { get; set; }
				[JsonProperty("24pct")]
				public decimal twentyFourPct { get; set; }
				public string timeToSell { get; set; }
				public string result { get; set; }
				public string updated { get; set; }
				public string updatedhtml { get; set; }
				public string sparkurl { get; set; }
				public string region { get; set; }
			}

			public class Region
			{
				public const string NA = "NA";
				public const string EU = "EU";
				public const string GB = "GB";
				public const string CN = "CN";
				public const string TW = "TW";
				public const string KR = "KR";

				public int timestamp { get; set; }
				public Raw raw { get; set; }
				public Formatted formatted { get; set; }
			}

			public class Response
			{
				public Region NA { get; set; }
				public Region EU { get; set; }
				public Region GB => EU;
				public Region CN { get; set; }
				public Region TW { get; set; }
				public Region KR { get; set; }

				public Region GetRegion(string region)
				{
					switch (region.ToUpper())
					{
						case "NA": return NA;
						case "EU": return EU;
						case "CN": return CN;
						case "TW": return TW;
						case "KR": return KR;
						case "GB": return GB;
						default: return NA;
					}
				}
		}

			#endregion Response Objects

			public static async Task<Response> GetSnapshot()
			{
				return await bnet.Networking.Http.Common.RequestAndDeserialize<Response>(apiUri, bnet.Networking.Http.CacheMode.Uncached);
			}
		}
	}
}
