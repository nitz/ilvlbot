using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bnet.Networking.Http
{
	internal class Cache
	{
		// todo -- make this a bit fancier of a cache :)
		private Dictionary<string, object> cache = new Dictionary<string, object>();

		public void CacheObject<T>(string key, T value)
		{
			cache[key] = value;
		}

		public bool TryGetCachedObject<T>(string key, out T value)
		{
			object o;
			bool was_cached = cache.TryGetValue(key, out o);

			if (was_cached && o != null && o is T)
			{
				value = (T)o;
			}
			else if (o != null && o is T == false)
			{
				throw new InvalidCastException($"Cache found an object for key, but it wasn't the type expected. (Expected: {typeof(T).Name}, found: {o.GetType().Name}.)");
			}
			else
			{
				value = default(T);
			}

			return was_cached;
		}
	}
}
