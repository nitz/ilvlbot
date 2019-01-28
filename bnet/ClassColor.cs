using System.Collections.Generic;

namespace bnet
{
	public class ClassColor
	{
		// publicly visible values
		public int ClassId { get; }
		public string ClassName { get; }
		public byte Red { get; }
		public byte Green { get; }
		public byte Blue { get; }

		// maps for conversions to and from names/ids
		private static readonly Dictionary<int, ClassColor> idMap = new Dictionary<int, ClassColor>();
		private static readonly Dictionary<string, ClassColor> nameMap = new Dictionary<string, ClassColor>();

		// color values via http://wowwiki.wikia.com/wiki/Class_colors
		// id numbers via https://us.api.battle.net/wow/data/character/classes
		public static readonly ClassColor DeathKnight = new ClassColor(6, "Death Knight", 196, 30, 59); // #C41F3B Red
		public static readonly ClassColor DemonHunter = new ClassColor(12, "Demon Hunter", 163, 48, 201); // #A330C9 Strong Magenta
		public static readonly ClassColor Druid = new ClassColor(11, "Druid", 255, 125, 10); // #FF7D0A Orange
		public static readonly ClassColor Hunter = new ClassColor(3, "Hunter", 171, 212, 115); // #ABD473 Green
		public static readonly ClassColor Mage = new ClassColor(8, "Mage", 105, 204, 240); // #69CCF0 Light blue
		public static readonly ClassColor Monk = new ClassColor(10, "Monk", 0, 255, 150); // #00FF96 Jade green
		public static readonly ClassColor Paladin = new ClassColor(2, "Paladin", 245, 140, 186); // #F58CBA Pink
		public static readonly ClassColor Priest = new ClassColor(5, "Priest", 255, 255, 255); // #FFFFFF White
		public static readonly ClassColor Rogue = new ClassColor(4, "Rogue", 255, 245, 105); // #FFF569 Light yellow
		public static readonly ClassColor Shaman = new ClassColor(7, "Shaman", 0, 112, 222); // #0070DE Blue
		public static readonly ClassColor Warlock = new ClassColor(9, "Warlock", 148, 130, 201); // #9482C9 Purple
		public static readonly ClassColor Warrior = new ClassColor(1, "Warrior", 199, 156, 110); // #C79C6E Tan

		// an 'uh-oh' case for failed conversions
		public static readonly ClassColor Unknown = new ClassColor(0, "Unknown", 255, 0, 255); // #FF00FF Magenta

		private ClassColor(int id, string name, byte r, byte g, byte b)
		{
			ClassId = id;
			ClassName = name;
			Red = r;
			Green = g;
			Blue = b;
			nameMap[ClassName.ToLower()] = this;
			idMap[ClassId] = this;
		}

		public override string ToString()
		{
			return ClassName;
		}

		public static implicit operator ClassColor(string name)
		{
			if (nameMap.TryGetValue(name.ToLower(), out ClassColor result))
				return result;
			else
				return Unknown;
		}

		public static implicit operator ClassColor(int id)
		{
			if (idMap.TryGetValue(id, out ClassColor result))
				return result;
			else
				return Unknown;
		}
	}
}
