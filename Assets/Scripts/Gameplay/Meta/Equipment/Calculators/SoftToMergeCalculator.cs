using System;

namespace Bones.Gameplay.Meta.Equipment
{
	public static class SoftToMergeCalculator
	{
		private static int[] _softValueToMergeMap;

		public static void Initialize(int[] softValueToMergeMap)
		{
			_softValueToMergeMap = softValueToMergeMap;
		}

		public static float NeedToMerge(Rare rare, int rareLevel)
		{
			if (_softValueToMergeMap == null)
				throw new InvalidOperationException("Soft to merge map not initialized");
			if (rare == Rare.None)
				throw new ArgumentException($"Invalid rare");
			if (rareLevel < 1 || rareLevel > IEquipment.MAX_RARE_LEVEL)
				throw new ArgumentException($"Invalid rare level");

			int index = ((int)rare - 1) * IEquipment.MAX_RARE_LEVEL + rareLevel - 1;
			return _softValueToMergeMap[index];
		}
	}
}
