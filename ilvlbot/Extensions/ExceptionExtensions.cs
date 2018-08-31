using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ilvlbot.Extensions
{
	public static class ExceptionExtensions
	{
		public static string GetCallingSite(this Exception ex)
		{
			return ex.StackTrace.Remove(ex.StackTrace.IndexOf("\r\n")).Trim();
		}
	}
}
