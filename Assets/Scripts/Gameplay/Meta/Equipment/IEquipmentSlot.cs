namespace Bones.Gameplay.Meta.Equipment
{
	public interface IEquipmentSlot
	{
		int Level { get; }
		EquipmentType Type { get; }
		int SoftNeedToUpgrade { get; }
		int RecipesNeedToUpgrade { get; } 
	}
}