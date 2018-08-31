using System;
using System.Net;

namespace bnet
{
	public class HttpRequestResult<T> where T : class
	{
		public bool Successful
		{
			get { return string.IsNullOrEmpty(ErrorText); }
		}

		public T Result;
		public string ErrorText;
		public Exception Exception;
		public HttpStatusCode StatusCode;

		public HttpRequestResult(T res, HttpStatusCode status)
		{
			Result = res;
			ErrorText = null;
			Exception = null;
			StatusCode = status;
		}

		public HttpRequestResult(string error, HttpStatusCode status, Exception ex = null)
		{
			ErrorText = error;
			Exception = ex;
			Result = null;
			StatusCode = status;
		}

		public static implicit operator T(HttpRequestResult<T> res)
		{
			if (res.Successful)
			{
				return res.Result;
			}
			else if (res.Exception != null)
			{
				throw res.Exception;
			}
			else
			{
				throw new InvalidCastException($"{typeof(HttpRequestResult<T>).Name} couldn't be cast to a {typeof(T).Name}: the HTTP request had failed: {res.ErrorText}");
			}
		}
	}
}
