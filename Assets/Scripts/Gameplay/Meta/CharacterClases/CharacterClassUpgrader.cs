using Bones.Gameplay.Meta.Currencies;
using Bones.Gameplay.Spells.Classes;
using System;
using System.Linq;

namespace Bones.Gameplay.Meta.CharacterClases
{
	public class CharacterClassUpgrader
	{
		private ICharacterClassStorage _characterClassStorage;
		private Currency _silver;

		public CharacterClassUpgrader(ICurrencyStorage currencyStorage,
			ICharacterClassStorage characterClassStorage)
		{
			_characterClassStorage = characterClassStorage;
			_silver = currencyStorage.Get(CurrencyType.Silver);
		}

		public bool EnoughSilverToRareUpgrade(ClassName name)
		{
			var checkingCharacterClass = GetCharacterClass(name);
			return checkingCharacterClass.SoftToUpgradeRare <= _silver.Value;
		}

		public void UpgradeRare(ClassName name)
		{
			var upgradingClass = GetCharacterClass(name);
			if (upgradingClass.Copies < upgradingClass.CopiesToUpgradeRare)
				throw new InvalidOperationException($"Not enough copies to upgrade {name}. Need: {upgradingClass.CopiesToUpgradeRare}; Available: {upgradingClass.Copies}");
			if (_silver.Value < upgradingClass.SoftToUpgradeRare)
				throw new InvalidOperationException($"Not enough {_silver.Type} to upgrade rare {name}");

			_silver.Withdrow(upgradingClass.SoftToUpgradeRare);
			(upgradingClass as CharacterClass).UpgradeRare();
			(_characterClassStorage as CharacterClassStorage).Upgrade();
		}

		public bool EnoughSilverToLevelUp(ClassName name)
		{
			var checkingCharacterClass = GetCharacterClass(name);
			return checkingCharacterClass.SoftToLevelUp <= _silver.Value;
		}

		public void UpLevel(ClassName name)
		{
			var upgradingClass = GetCharacterClass(name);
			if (upgradingClass.LevelUpProgress < ICharacterClass.READY_TO_LEVEL_UP)
				throw new InvalidOperationException($"Not enough experience to level up. Need: {ExperienceToLevelUpCalculator.NeedExperienceToLevelUp(upgradingClass.Level)}, Current: {upgradingClass.Experience.Value}");
			if (_silver.Value < upgradingClass.SoftToLevelUp)
				throw new InvalidOperationException($"Not enough {_silver.Type} to level up {name}");

			_silver.Withdrow(upgradingClass.SoftToLevelUp);
			(upgradingClass as CharacterClass).UpLevel();
			(_characterClassStorage as CharacterClassStorage).Upgrade();
		}

		private ICharacterClass GetCharacterClass(ClassName name)
		{
			var upgradingClass = _characterClassStorage.ActiveClasses.First(x => x.Name == name);
			if (upgradingClass == null)
				throw new InvalidOperationException($"Character class {name} isn't opened.");
			return upgradingClass;
		}
	}
}
