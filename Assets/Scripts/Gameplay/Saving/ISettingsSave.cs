namespace Bones.Gameplay.Saving
{
	public interface ISettingsSave
	{
		int CurrentLevel { get; }
		int MaxUnlockedLevel { get; }
	}
}
