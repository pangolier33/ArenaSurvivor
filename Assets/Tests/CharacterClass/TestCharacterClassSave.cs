using Bones.Gameplay.Meta.CharacterClases;
using Bones.Gameplay.Meta.Equipment;
using Bones.Gameplay.Spells.Classes;

namespace Assets.Tests
{
	public class TestCharacterClassSave : ICharacterClassSave
	{
		public ClassName Name { get; init; }

		public float Experience { get; init; }

		public Rare Rare { get; init; }

		public int Copies { get; init; }

		public int Level { get; init; }
	}
}
