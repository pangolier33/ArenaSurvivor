using System;

namespace Bones.Gameplay.Meta.CharacterClases
{
	public static class SoftToLevelUpCalculator
	{
		private static int[] _softValueToLevelUpMap;

		public static void Initialize(int[] softValueToLevelUpMap)
		{
			_softValueToLevelUpMap = softValueToLevelUpMap;
		}

		public static float NeedToLevelUp(int level)
		{
			if (_softValueToLevelUpMap == null)
				throw new InvalidOperationException("Soft value to level up map not initialized!");
			if (level < 1 || level + 1 >= _softValueToLevelUpMap.Length)
				throw new ArgumentOutOfRangeException(nameof(level));
			return _softValueToLevelUpMap[level - 1];
		}
	}
}
