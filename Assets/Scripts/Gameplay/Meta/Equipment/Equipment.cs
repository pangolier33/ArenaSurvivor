using Bones.Gameplay.Players;
using System;

namespace Bones.Gameplay.Meta.Equipment
{
	public class Equipment : IEquipment
	{
		public string Id => Data.Id;
		public EquipmentType Type => Data.Type;
		public Rare Rare { get; private set; }
		public int RareLevel { get; private set; }
		public IEquipmentData Data { get; }
		public float NeedSilverToMerge => SoftToMergeCalculator.NeedToMerge(Rare, RareLevel);
		public float RareMultiplier => EquipmentRareMultiplierCalculator.GetRareMultiplier(Rare, RareLevel);
		public int Level { get; set; }
		public float LevelMultiplier => EquipmentLevelMultiplierCalculator.GetMultiplier(Level);
		public int Livetime { get; } = (int)(UnityEngine.Random.value * 80 + 10);
		public int Durability { get; } = (int)(UnityEngine.Random.value * 80 + 10);

		public Equipment(IEquipmentData equipmentData)
		{
			Data = equipmentData;
			Rare = Rare.Common;
			RareLevel = 1;
		}

		public Equipment(IEquipment original, Rare rare, int rareLevel)
		{
			Data = original.Data;
			Rare = rare;
			RareLevel = rareLevel;
		}

		public void IncreaseRare()
		{
			if (Rare == Rare.Draconic && RareLevel == IEquipment.MAX_RARE_LEVEL)
				throw new InvalidOperationException("Already max rare.");
			if (RareLevel == IEquipment.MAX_RARE_LEVEL)
			{
				Rare = (Rare)((int)Rare + 1);
				RareLevel = 1;
			}
			else
			{
				RareLevel++;
			}
		}

		public void Apply(Player player)
		{
			Data.ApplyStats(LevelMultiplier * RareMultiplier, player);
		}
	}
}
