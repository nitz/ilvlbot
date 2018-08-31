using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace bnet.Requests.Fields
{
	public class Character
	{
		public bool Achievements { get; set; } = false;
		public bool Appearance { get; set; } = false;
		public bool Feed { get; set; } = false;
		public bool Items { get; set; } = false;
		public bool Mounts { get; set; } = false;
		public bool Progression { get; set; } = false;
		public bool Talents { get; set; } = false;
		public bool Titles { get; set; } = false;
		public bool Stats { get; set; } = false;
		public bool Statistics { get; set; } = false;
		public bool Guild { get; set; } = false;

		/// <summary>
		/// A basic, minimal info return.
		/// </summary>
		public static Character Minimal { get { return new Character(false); } }

		/// <summary>
		/// A slimmed down set of fields that still includes some helpful things, without the 'heavy' lifting.
		/// </summary>
		public static Character Slim { get { return new Character() { Appearance = true, Items = true, Stats = true, Talents = true, Guild = true }; } }

		/// <summary>
		/// A full set of fields -- get it all!
		/// </summary>
		public static Character Full { get { return new Character(true); } }
		
		/// <summary>
		/// Default ctor.
		/// </summary>
		public Character()
		{

		}

		/// <summary>
		/// A ctor to set all or none of the fields.
		/// </summary>
		/// <param name="full_character"></param>
		public Character(bool full_character)
		{
			if (full_character)
			{
				var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => x.PropertyType == typeof(bool));
				foreach (var prop in properties)
					prop.SetValue(this, true);
			}
		}

		/// <summary>
		/// Builds a query string paramater based on the fields selected.
		/// </summary>
		/// <returns></returns>
		internal string ToQueryParam()
		{
			var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => x.PropertyType == typeof(bool));
			var param = string.Join(",", properties.Where(x => (bool)x.GetValue(this)).Select(x => x.Name.ToLower()).OrderBy(x => x));
			return param;
		}

		public override string ToString()
		{
			return ToQueryParam();
		}
	}
}
