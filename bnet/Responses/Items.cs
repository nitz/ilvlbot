using System.Collections.Generic;

namespace bnet.Responses
{
	public class Items
	{
		public int averageItemLevel { get; set; }
		public int averageItemLevelEquipped { get; set; }
		public Head head { get; set; }
		public Neck neck { get; set; }
		public Shoulder shoulder { get; set; }
		public Back back { get; set; }
		public Chest chest { get; set; }
		public Tabard tabard { get; set; }
		public Wrist wrist { get; set; }
		public Hands hands { get; set; }
		public Waist waist { get; set; }
		public Legs legs { get; set; }
		public Feet feet { get; set; }
		public Finger finger1 { get; set; }
		public Finger finger2 { get; set; }
		public Trinket trinket1 { get; set; }
		public Trinket trinket2 { get; set; }
		public MainHand mainHand { get; set; }
		public OffHand offHand { get; set; }

		public IEnumerable<Item> allItems
		{
			get
			{
				Item[] arr = new Item[] { head, neck, shoulder, back, chest, tabard, wrist, hands, waist, legs, feet, finger1, finger2, trinket1, trinket2, mainHand, offHand };

				foreach (var i in arr)
					if (i != null)
						yield return i;
			}
		}

		public float calculatedItemLevel
		{
			get
			{
				float total = 0;

				total += head?.itemLevel ?? 0;
				total += neck?.itemLevel ?? 0;
				total += shoulder?.itemLevel ?? 0;
				total += back?.itemLevel ?? 0;
				total += chest?.itemLevel ?? 0;
				// total += chest?.itemLevel ?? 0; // no tabard ilvl please
				total += wrist?.itemLevel ?? 0;
				total += hands?.itemLevel ?? 0;
				total += waist?.itemLevel ?? 0;
				total += legs?.itemLevel ?? 0;
				total += feet?.itemLevel ?? 0;
				total += finger1?.itemLevel ?? 0;
				total += finger2?.itemLevel ?? 0;
				total += trinket1?.itemLevel ?? 0;
				total += trinket2?.itemLevel ?? 0;
				total += mainHand?.itemLevel ?? 0;

				// I'm not sure how to tell if mainhand is a 2h weapon or not.
				// I'll just make the assumption that if they don't have an offhand,
				// The mainhand is actually a 2-hander, and to count it's ilvl twice.
				total += offHand?.itemLevel ?? (mainHand?.itemLevel ?? 0);

				return total / 16;
			}
		}

		public int legendaryItemCount
		{
			get
			{
				int count = 0;
				foreach (var i in allItems)
					if (i.quality == ItemQuality.Legendary)
						count++;
				return count;
			}
		}

		public int artifactRank
		{
			get
			{
				if (mainHand?.quality == ItemQuality.Artifact && mainHand.artifactRank > 0)
					return mainHand.artifactRank;
				else if (offHand?.quality == ItemQuality.Artifact && offHand.artifactRank > 0)
					return offHand.artifactRank;
				return 0;
			}
		}
	}
}
