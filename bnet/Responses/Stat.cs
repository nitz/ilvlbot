
namespace bnet.Responses
{
	public class Stat
	{
		public enum Type
		{
			None = -1,
			Mana = 0,
			Health = 1,
			Agility = 3,
			Strenght = 4,
			Intellect = 5,
			Spirit = 6,
			Stamina = 7,

			DefenseSkill = 12,
			Dodge = 13,
			Parry = 14,
			Block = 15,
			MeleeHit = 16,
			RangedHit = 17,
			SpellHit = 18,
			MeleeCrit = 19,
			RangedCrit = 20,
			SpellCrit = 21,
			MeleeHitTaken = 22,
			RangedHitTaken = 23,
			SpellHitTaken = 24,
			MeleeCritTaken = 25,
			RangedCritTaken = 26,
			SpellCritTaken = 27,
			MeleeHaste = 28,
			RangedHaste = 29,
			SpellHaste = 30,
			Hit = 31,
			Crit = 32,
			HitTaken = 33,
			CritTaken = 34,
			Resilience = 35,
			Haste = 36,
			Expertise = 37,
			AttackPower = 38,
			RangedAttackPower = 39,
			Versatility = 40,
			SpellHealingDone = 41,          // deprecated
			SpellDamageDone = 42,           // deprecated
			ManaRegeneration = 43,
			ArmorPenetration = 44,
			SpellPower = 45,
			HealthRegen = 46,
			SpellPenetration = 47,
			BlockValue = 48,
			Mastery = 49,
			BonusArmor = 50,
			FireResistance = 51,
			FrostResistance = 52,
			HolyResistance = 53,
			ShadowResistance = 54,
			NatureResistance = 55,
			ArcaneResistance = 56,
			PVPPower = 57,

			Multistrike = 59,
			Readiness = 60,
			Speed = 61,
			Leech = 62,
			Avoidence = 63,
			Indestructible = 64,
			WOD_5 = 65,
			WOD_6 = 66,

			StrenghtAgilityIntelect = 71,
			StrenghtAgility = 72,
			AgilityIntelect = 73,
			StrenghtIntelect = 74,
		};

		public Type stat { get; set; }
		public int amount { get; set; }
	}
}
