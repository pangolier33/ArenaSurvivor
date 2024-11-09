using System;

namespace Bones.Gameplay.Meta.CharacterClases
{
	public static class ExperienceToLevelUpCalculator
	{
		private static int[] _experienceClassMap;

		public static void Initialize(int[] experienceClassMap)
		{
			if (experienceClassMap == null || experienceClassMap.Length == 0)
				throw new ArgumentException(nameof(experienceClassMap));
			_experienceClassMap = experienceClassMap;
		}

		public static int EnoughForTheLevel(float experience)
		{
			if (_experienceClassMap == null)
				throw new InvalidOperationException("Experience map not initialized!");
			int level = 0;
			while (level < _experienceClassMap.Length && _experienceClassMap[level] <= experience)
				level++;

			return level + 1;
		}

		public static int NeedExperienceToLevelUp(int level)
		{
			if (_experienceClassMap == null)
				throw new InvalidOperationException("Experience map not initialized!");
			return _experienceClassMap[level];
		}

	}
}
