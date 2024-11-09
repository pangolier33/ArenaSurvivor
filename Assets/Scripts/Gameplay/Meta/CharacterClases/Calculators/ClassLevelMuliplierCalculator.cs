using System;

namespace Bones.Gameplay.Meta.CharacterClases
{
	public static class ClassLevelMuliplierCalculator
	{
		private static float[] _multiplierOfClassLevelMap;

		public static void Initialize(float[] damageMultiplierOfClassLevelMap)
		{
			_multiplierOfClassLevelMap = damageMultiplierOfClassLevelMap;
		}

		public static float GetMultiplier(int level)
		{
			if (_multiplierOfClassLevelMap == null)
				throw new InvalidOperationException("Damage multiplier map not initialized!");
			if (level < 1 || level + 1 >= _multiplierOfClassLevelMap.Length)
				throw new ArgumentOutOfRangeException(nameof(level));
			return _multiplierOfClassLevelMap[level - 1];
		}
	}
}
