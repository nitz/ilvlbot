using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace bnet
{
	public static class NetworkCommon
	{
		// todo -- make this a bit fancier of a cache :)
		public enum CacheMode { Uncached, Cached };
		private static Dictionary<string, string> cache = new Dictionary<string, string>();

		// a list of strings (eg. 'apikey') that we don't want to throw in an error message and sorts.
		// Scrub() will remove any instance of the found string and anything after it.
		private static List<string> dontPrintStrings = new List<string>();

		public static void AddForbiddenString(string s)
		{
			dontPrintStrings.Add(s);
		}

		public static HttpRequestResult<T> RequestAndDeserialize<T>(string request, CacheMode cache_mode = CacheMode.Uncached) where T : class
		{
			try
			{
				HttpStatusCode status = HttpStatusCode.Continue;
				string response_text = null;

				// if we're a cachable request, see if we have it cached, if so, use that.
				if (cache_mode == CacheMode.Cached)
				{
					if (cache.ContainsKey(request))
					{
						status = HttpStatusCode.OK;
						response_text = cache[request];
					}
				}

				if (status != HttpStatusCode.OK)
				{
					//string printsafe_request = request.Replace(ApiKey.Key, "(removed)");
					//Api.Log($"Request: {printsafe_request}");
					WebRequest req = WebRequest.CreateHttp(request);

					if (!(req.GetResponse() is HttpWebResponse resp))
						return null;

					status = resp.StatusCode;

					StreamReader reader = new StreamReader(resp.GetResponseStream());
					response_text = reader.ReadToEnd();

					if (cache_mode == CacheMode.Cached && status == HttpStatusCode.OK)
					{
						cache[request] = response_text;
					}
				}

				if (status == HttpStatusCode.OK)
				{
					var result = JsonConvert.DeserializeObject<T>(response_text);
					return new HttpRequestResult<T>(result, status);
				}
				else
				{
					Api.Log($"Error ({status}): {response_text}");
					return new HttpRequestResult<T>(response_text, status, null);
				}
			}
			catch (WebException wex)
			{
				string failure = $"WebException for request: {Scrub(request)}\r\nReason: {wex.Message}";
				Api.Log(failure);
				HttpStatusCode status = (wex.Response is HttpWebResponse) ? (wex.Response as HttpWebResponse).StatusCode : HttpStatusCode.BadRequest;
				return new HttpRequestResult<T>(failure, status, wex);
			}
			catch (Exception ex)
			{
				string failure = $"Exception for request: {Scrub(request)}\r\nReason: {ex.Message}";
				Api.Log(failure);
				return new HttpRequestResult<T>(failure, HttpStatusCode.BadRequest, ex);
			}
		}

		private static string Scrub(string req)
		{
			string ret = req;
			foreach (string forbidden in dontPrintStrings)
			{
				int forbidden_text_location = ret.IndexOf(forbidden);
				if (forbidden_text_location >= 0)
					ret = ret.Remove(forbidden_text_location);
			}

			return ret;
		}
	}
}
