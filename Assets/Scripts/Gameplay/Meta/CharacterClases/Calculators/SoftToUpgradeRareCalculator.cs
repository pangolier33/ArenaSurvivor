using Bones.Gameplay.Meta.Equipment;
using System;

namespace Bones.Gameplay.Meta.CharacterClases
{
	public static class SoftToUpgradeRareCalculator
	{
		private static int[] _softValueToUpgradeRare;

		public static void Initialize(int[] softValueToUpgradeRare)
		{
			_softValueToUpgradeRare = softValueToUpgradeRare;
		}

		public static int NeedToUpgradeRare(Rare currentRare)
		{
			if (_softValueToUpgradeRare == null)
				throw new InvalidOperationException("Soft value to upgrade rare not initialized!");
			int rareIndex = (int)currentRare - 1;
			if (rareIndex >= _softValueToUpgradeRare.Length)
				throw new ArgumentOutOfRangeException(nameof(currentRare));
			return _softValueToUpgradeRare[rareIndex];
		}
	}
}
