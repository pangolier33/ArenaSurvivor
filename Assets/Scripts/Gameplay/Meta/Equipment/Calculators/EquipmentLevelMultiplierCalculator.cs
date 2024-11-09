using System;

namespace Bones.Gameplay.Meta.Equipment
{
	public static class EquipmentLevelMultiplierCalculator
	{
		private static float[] _equipmentLevelMultiplierMap;

		public static void Initialize(float[] equipmentLevelMultiplierMap)
		{
			_equipmentLevelMultiplierMap = equipmentLevelMultiplierMap;
		}

		public static float GetMultiplier(int level)
		{
			if (_equipmentLevelMultiplierMap == null)
				throw new InvalidOperationException($"{nameof(_equipmentLevelMultiplierMap)} not initialized");
			return _equipmentLevelMultiplierMap[level];
		}
	}
}
