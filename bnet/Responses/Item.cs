using System.Collections.Generic;

namespace bnet.Responses
{
	public class Item
	{
		public int id { get; set; }
		public string name { get; set; }
		public string icon { get; set; }
		public ItemQuality quality { get; set; }
		public int itemLevel { get; set; }
		public TooltipParams tooltipParams { get; set; }
		public List<Stat> stats { get; set; }
		public int armor { get; set; }
		public string context { get; set; }
		public List<int> bonusLists { get; set; }
		public int artifactId { get; set; }
		public int displayInfoId { get; set; }
		public int artifactAppearanceId { get; set; }
		public List<ArtifactTrait> artifactTraits { get; set; }
		public List<Relic> relics { get; set; }
		public Appearance appearance { get; set; }
		public AzeriteItem azeriteItem { get; set; }
		public AzeriteEmpoweredItem azeriteEmpoweredItem { get; set; }

		public int artifactRank
		{
			get
			{
				return totalArtifactRank - totalRelicTraitRank;
			}
		}

		public int totalArtifactRank
		{
			get
			{
				if (artifactTraits == null || artifactTraits.Count == 0)
					return 0;

				int relic_ranks = totalRelicTraitRank;

				int total = 0;

				foreach (var trait in artifactTraits)
				{
					total += trait.rank;
				}

				return total;
			}
		}

		public int totalRelicTraitRank
		{
			get
			{
				if (relics == null || relics.Count == 0)
					return 0;

				int total = 0;

				foreach (var relic in relics)
				{
					// #hack #todo -- Don't know how to handle this now,
					// I can't figure out how the relic modifies ranks.
					//
					// If I look up the item, it has a 'gemInfo' that
					// describes a 'Relic Enhancement' (as well as the type
					// of relic it actually is,) but I can't tell if it's 
					// describing the trait it modifies.
					//
					// For future me, here is a fel relic, 'Writ of Subjugation'
					// https://us.api.battle.net/wow/item/140824?locale=en_US&apikey=[key]
					// and here it is with my character's bonus lists.
					// the ilvl gets modified, as does the context.
					// https://us.api.battle.net/wow/item/140824?locale=en_US&apikey=[key]&bl=3514,1472,1813
					//
					// But even with all that, I still don't know the relation to traits.

					// For now, just add one rank per relic.
					total++;
				}

				return total;
			}
		}

		public bool isAzeriteItem
		{
			get
			{
				return azeriteItem?.azeriteLevel > 0;
			}
		}

		public int azeriteLevel
		{
			get
			{
				if (isAzeriteItem)
					return azeriteItem.azeriteLevel;
				return 0;
			}
		}
	}
}
