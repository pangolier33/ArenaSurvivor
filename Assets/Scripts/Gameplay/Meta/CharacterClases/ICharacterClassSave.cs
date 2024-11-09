using Bones.Gameplay.Meta.Equipment;
using Bones.Gameplay.Spells.Classes;

namespace Bones.Gameplay.Meta.CharacterClases
{
	public interface ICharacterClassSave
	{
		public ClassName Name { get; }
		public float Experience { get; }
		public Rare Rare { get; }
		public int Copies { get; }
		public int Level { get; }
	}
}
