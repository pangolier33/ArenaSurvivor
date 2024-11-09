using System;

namespace Bones.Gameplay.Saving.Local
{
	[Serializable]
	public class LocalSettingsSave : ISettingsSave
	{
		public int _selectedLevel;
		public int _lastUnlockedLevel;

		public int CurrentLevel { get => _selectedLevel; set => _selectedLevel = value; }

		public int MaxUnlockedLevel { get => _lastUnlockedLevel; set => _lastUnlockedLevel = value; }
	}
}
