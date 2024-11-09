namespace Bones.Gameplay.Spells.Classes
{
	public interface ISpellBranch
	{
		bool IsActive { get; }
		bool IsAvailable { get; }
		ISpellModel Model { get; }
	}
}