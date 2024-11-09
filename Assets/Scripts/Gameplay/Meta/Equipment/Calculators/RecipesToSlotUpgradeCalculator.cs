using System;

namespace Bones.Gameplay.Meta.Equipment
{
	public static class RecipesToSlotUpgradeCalculator
	{
		private static int[] _recipesToSlotUpgradeMap;

		public static void Initialize(int[] recipesToSlotUpgradeMap)
		{
			_recipesToSlotUpgradeMap = recipesToSlotUpgradeMap;
		}

		public static int NeedToUpgrade(int level)
		{
			if (_recipesToSlotUpgradeMap == null)
				throw new InvalidOperationException($"{_recipesToSlotUpgradeMap} not initialized");
			return _recipesToSlotUpgradeMap[level];
		}
	}
}
