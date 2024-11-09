using Bones.Gameplay.Meta.Equipment;
using System;

namespace Bones.Gameplay.Meta.CharacterClases
{
	public static class CopiesToUpgradeRareCalculator
	{

		private static int[] _copiesToRareUpgradeMap;

		public static void Initialize(int[] copiesToRareUpgradeMap)
		{
			_copiesToRareUpgradeMap = copiesToRareUpgradeMap;
		}

		public static int NeedToUpgradeRare(Rare rare)
		{
			if (_copiesToRareUpgradeMap == null)
				throw new InvalidOperationException("Copies to upgrade rare not initialized!");
			int rareIndex = (int)rare - 1;
			if (rareIndex >= _copiesToRareUpgradeMap.Length)
				throw new ArgumentOutOfRangeException(nameof(rare));
			return _copiesToRareUpgradeMap[rareIndex];
		}
	}
}
