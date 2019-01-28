using System;
using System.Collections.Generic;

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
			bool was_cached = cache.TryGetValue(key, out object o);

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
				value = default;
			}

			return was_cached;
		}
	}
}
