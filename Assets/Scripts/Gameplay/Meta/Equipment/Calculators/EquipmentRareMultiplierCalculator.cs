using System;

namespace Bones.Gameplay.Meta.Equipment
{
	public static class EquipmentRareMultiplierCalculator
	{
		private static float[] _rareMultiplierMap;

		public static void Initialize(float[] rareMultiplierMap)
		{
			_rareMultiplierMap = rareMultiplierMap;
		}

		public static float GetRareMultiplier(Rare rare, int rareLevel)
		{
			if (_rareMultiplierMap == null)
				throw new InvalidOperationException("Rare multiplier map not initialized");
			if (rare == Rare.None)
				throw new ArgumentException($"Invalid rare");
			if (rareLevel < 1 || rareLevel > IEquipment.MAX_RARE_LEVEL)
				throw new ArgumentException($"Invalid rare level {rareLevel}");

			int index = ((int)rare - 1) * IEquipment.MAX_RARE_LEVEL + rareLevel - 1;
			return _rareMultiplierMap[index];
		}
	}
}
