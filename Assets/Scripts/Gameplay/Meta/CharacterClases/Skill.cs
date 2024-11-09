using Bones.Gameplay.Spells.Classes;
using UnityEngine;
using static Bones.Gameplay.Spells.Classes.ClassContainer;

namespace Bones.Gameplay.Meta.CharacterClases
{
	public class Skill
	{
		public ISpellBranch SpellBranch { get; }
		public SpellType Type { get; }
		public string Name => SpellBranch.Model.Name;
		public string Description => SpellBranch.Model.Description;
		public Sprite Icon => SpellBranch.Model.Icon;
		public int Levels
		{
			get
			{
				switch (Type)
				{
					case SpellType.Base:
						return 3;
					case SpellType.Common:
						return 5;
					case SpellType.Ult:
						return 3;
					default:
						throw new System.Exception($"Unknown spell type {Type}");
				}
			}
		}

		public Skill(ISpellBranch spellBranch, SpellType type)
		{
			SpellBranch = spellBranch;
			Type = type;
		}
	}
}
