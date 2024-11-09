using Bones.Gameplay.Meta.Currencies;
using Bones.Gameplay.Meta.Equipment;
using Bones.Gameplay.Spells.Classes;
using System.Collections.Generic;
using UnityEngine;

namespace Bones.Gameplay.Meta.CharacterClases
{
	public interface ICharacterClass
	{
		public const float READY_TO_LEVEL_UP = 1.0f;

		int Copies { get; }
		int CopiesToUpgradeRare { get; }
		int SoftToUpgradeRare { get; }
		Currency Experience { get; }
		float ExperienceToNextLevel { get; }
		Sprite Icon { get; }
		int Level { get; }
		float LevelUpProgress { get; }
		float SoftToLevelUp { get; }
		float DamageMultiplier { get; }
		float NextLevelDamageMultiplier { get; }
		ClassName Name { get; }
		Rare Rare { get; }
		IEnumerable<Skill> Skills { get; }
		IClassContainer ClassContainer { get; }

		bool ReadyToUpgradeRare { get; }
		bool ReadyToLevelUp { get; }
	}
}