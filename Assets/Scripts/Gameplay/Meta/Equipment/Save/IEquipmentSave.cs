namespace Bones.Gameplay.Meta.Equipment
{
	public interface IEquipmentSave
	{
		string Id { get; }
		Rare Rare { get; }
		int RareLevel { get; }
	}
}
