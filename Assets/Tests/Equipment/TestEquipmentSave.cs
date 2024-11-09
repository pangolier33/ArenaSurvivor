using Bones.Gameplay.Meta.Equipment;

namespace Assets.Tests
{
	internal class TestEquipmentSave : IEquipmentSave
	{
		public string Id { get; init; }

		public Rare Rare { get; init; }

		public int RareLevel { get; init; }
	}
}
