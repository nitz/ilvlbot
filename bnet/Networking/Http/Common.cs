using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace bnet.Networking.Http
{
	public static class Common
	{
		private static HttpClient client;
		private static Cache cache = new Cache();

		// a list of strings (eg. 'apikey') that we don't want to throw in an error message and sorts.
		// Scrub() will remove any instance of the found string and anything after it.
		private static List<string> dontPrintStrings = new List<string>();

		static Common()
		{
			// according to https://aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/,
			// and https://github.com/mspnp/performance-optimization/blob/master/ImproperInstantiation/docs/ImproperInstantiation.md
			// apparently I shouldn't be creating a new HttpClient for each request.
			client = new HttpClient();
		}

		public static void AddForbiddenString(string s)
		{
			dontPrintStrings.Add(s);
		}

		public static async Task<RequestResult<T>> RequestAndDeserialize<T>(string request, CacheMode cache_mode = CacheMode.Uncached, HttpContent postContent = null, params KeyValuePair<string, string>[] additionalHeaders) where T : class
		{
			try
			{
				HttpStatusCode status = HttpStatusCode.Continue;
				string response_text = null;

				// if we're a cachable request, see if we have it cached, if so, use that.
				if (cache_mode == CacheMode.Cached)
				{
					if (cache.TryGetCachedObject(request, out response_text))
					{
						status = HttpStatusCode.OK;
					}
				}

				if (status != HttpStatusCode.OK)
				{
					//Api.Log($"Request: {Scrub(request)}");

					HttpResponseMessage resp = null;

					var method = (postContent == null) ? HttpMethod.Get : HttpMethod.Post;

					using (var hrm = new HttpRequestMessage(method, request))
					{
						if (additionalHeaders != null)
						{
							foreach (var p in additionalHeaders)
							{
								hrm.Headers.Add(p.Key, p.Value);
							}
						}

						if (postContent != null)
						{
							hrm.Content = postContent;
						}

						resp = await client.SendAsync(hrm);
					}

					if (resp == null)
					{
						return null;
					}

					status = resp.StatusCode;

					response_text = await resp.Content.ReadAsStringAsync();

					if (cache_mode == CacheMode.Cached && status == HttpStatusCode.OK)
					{
						cache.CacheObject(request, response_text);
					}
				}

				if (status == HttpStatusCode.OK)
				{
					var result = JsonConvert.DeserializeObject<T>(response_text);
					return new RequestResult<T>(result, status);
				}
				else
				{
					Api.Log($"Error ({status}): {response_text}");
					return new RequestResult<T>(response_text, status, null);
				}
			}
			catch (WebException wex)
			{
				string failure = $"WebException for request: {Scrub(request)}\r\nReason: {wex.Message}";
				Api.Log(failure);
				HttpStatusCode status = (wex.Response is HttpWebResponse) ? (wex.Response as HttpWebResponse).StatusCode : HttpStatusCode.BadRequest;
				return new RequestResult<T>(failure, status, wex);
			}
			catch (Exception ex)
			{
				string failure = $"Exception for request: {Scrub(request)}\r\nReason: {ex.Message}";
				Api.Log(failure);
				return new RequestResult<T>(failure, HttpStatusCode.BadRequest, ex);
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
