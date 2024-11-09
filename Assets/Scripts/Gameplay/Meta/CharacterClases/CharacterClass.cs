using Bones.Gameplay.Meta.Currencies;
using Bones.Gameplay.Meta.Equipment;
using Bones.Gameplay.Spells.Classes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bones.Gameplay.Meta.CharacterClases
{
	public class CharacterClass : ICharacterClass
	{
		public IClassContainer ClassContainer { get; }
		public ClassName Name => ClassContainer.Name;
		public Sprite Icon => ClassContainer.Icon;
		public IEnumerable<Skill> Skills => ClassContainer.GetSkils();
		public Currency Experience { get; }
		public float ExperienceToNextLevel => ExperienceToLevelUpCalculator.NeedExperienceToLevelUp(Level);
		public int Level { get; private set; }
		public float LevelUpProgress => Mathf.Clamp01(Experience.Value / ExperienceToLevelUpCalculator.NeedExperienceToLevelUp(Level));
		public float SoftToLevelUp => SoftToLevelUpCalculator.NeedToLevelUp(Level);
		public Rare Rare { get; private set; }
		public int Copies { get; private set; }
		public int CopiesToUpgradeRare => CopiesToUpgradeRareCalculator.NeedToUpgradeRare(Rare);
		public float DamageMultiplier => ClassLevelMuliplierCalculator.GetMultiplier(Level);
		public float NextLevelDamageMultiplier => ClassLevelMuliplierCalculator.GetMultiplier(Level + 1);
		public int SoftToUpgradeRare => SoftToUpgradeRareCalculator.NeedToUpgradeRare(Rare);
		public bool ReadyToUpgradeRare => Copies >= CopiesToUpgradeRare && Level % 10 == 0 && ((int)Rare) == Level / 10;
		public bool ReadyToLevelUp => Experience.Value >= ExperienceToNextLevel && (Level % 10 != 0 || ((int)Rare) != Level / 10);

		public CharacterClass(IClassContainer classContainer)
		{
			ClassContainer = classContainer;
			Rare = Rare.Common;
			Experience = new Currency(0, CurrencyType.Expirience);
			Level = 1;
		}

		public CharacterClass(ICharacterClass original, ICharacterClassSave save)
		{
			ClassContainer = original.ClassContainer;
			Rare = save.Rare;
			Copies = save.Copies;
			Level = save.Level;
			Experience = new Currency(save.Experience, CurrencyType.Expirience);
		}

		public CharacterClass(ICharacterClass original)
		{
			ClassContainer = original.ClassContainer;
			Rare = Rare.Common;
			Experience = new Currency(0, CurrencyType.Expirience);
		}

		public void UpgradeRare()
		{
			int copiesToNextRare = CopiesToUpgradeRareCalculator.NeedToUpgradeRare(Rare);
			if (Copies < copiesToNextRare)
				throw new InvalidOperationException($"Not enough copies to upgrade rare. Needs: {copiesToNextRare}, available: {Copies}");
			Copies -= copiesToNextRare;
			Rare = (Rare)((int)Rare + 1);
		}

		public void UpLevel()
		{
			if (Level >= ExperienceToLevelUpCalculator.EnoughForTheLevel(Experience.Value))
				throw new InvalidOperationException($"Not enough experience to level up. Need: {ExperienceToLevelUpCalculator.NeedExperienceToLevelUp(Level)}, Current: {Experience.Value}");
			Level++;
		}

		public void AddCopy()
		{
			Copies++;
		}

		public override string ToString()
		{
			return $"{Name} Level:{Level} Rare: {Rare} Level: {Level} Copies: {Copies}/{CopiesToUpgradeRare}";
		}
	}
}
