using System;

namespace Bones.Gameplay.Meta.Equipment
{
	public static class SoftToSlotUpgradeCalculator
	{
		private static int[] _softToSlotUpgradeMap;

		public static void Initialize(int[] softToSlotUpgradeMap)
		{
			_softToSlotUpgradeMap = softToSlotUpgradeMap;
		}

		public static int NeedToUpgrade(int level)
		{
			if (_softToSlotUpgradeMap == null)
				throw new InvalidOperationException("Soft to slot upgrade map not initialized");
			return _softToSlotUpgradeMap[level];
		}
	}
}
