using Bones.Gameplay.Meta.CharacterClases;
using Bones.Gameplay.Meta.Equipment;
using Bones.Gameplay.Spells.Classes;
using System;

namespace Bones.Gameplay.Saving.Local
{
	[Serializable]
	public class LocalCharacterClassSave : ICharacterClassSave
	{
		public ClassName _name;
		public float _expirience;
		public Rare _rare;
		public int _copies;
		public int _level;

		public ClassName Name { get => _name; set => _name = value; }
		public float Experience { get => _expirience; set => _expirience = value; }
		public Rare Rare { get => _rare; set => _rare = value; }
		public int Copies { get => _copies; set => _copies = value; }
		public int Level { get => _level; set => _level = value; }

		public LocalCharacterClassSave()
		{
		}

		public LocalCharacterClassSave(ICharacterClass characterClass)
		{
			Name = characterClass.Name;
			Experience = characterClass.Experience.Value;
			Rare = characterClass.Rare;
			Copies = characterClass.Copies;
			Level = characterClass.Level;
		}
	}
}
