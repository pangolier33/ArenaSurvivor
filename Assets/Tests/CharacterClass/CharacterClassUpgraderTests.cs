using Bones.Gameplay.Meta.CharacterClases;
using Bones.Gameplay.Meta.Currencies;
using Bones.Gameplay.Meta.Equipment;
using Bones.Gameplay.Spells.Classes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Tests
{
	public class CharacterClassUpgraderTests
	{
		[Test]
		public void EnoughSilverToRareUpgradeNegativeTest()
		{
			var baseClasses = new TestClassContainer[]
			{
				new TestClassContainer(new Skill[]{ }) { Name = ClassName.Mage },
				new TestClassContainer(new Skill[]{ }) { Name = ClassName.Warrior },
				new TestClassContainer(new Skill[]{ }) { Name = ClassName.Healer }
			};

			var currencyStorage = new CurrencyStorage();
			currencyStorage.ApplySave(new ICurrencySave[] { new TestCurrencySave { Type = CurrencyType.Silver, Value = 0 } });
			var characterClassStorage = new CharacterClassStorage(baseClasses);
			var save = new TestCharacterClassStorageSave()
			{
				ClassesSave = new List<ICharacterClassSave>()
				{
					new TestCharacterClassSave() { Name = ClassName.Warrior, Copies = 0, Experience = 0, Rare = Rare.Common }
				},
				ActiveClassesSave = new ClassName[0]
			};
			characterClassStorage.ApplySave(save);
			SoftToUpgradeRareCalculator.Initialize(new[] { 100 });
			var upgrader = new CharacterClassUpgrader(currencyStorage, characterClassStorage);
			Assert.AreEqual(false, upgrader.EnoughSilverToRareUpgrade(ClassName.Warrior));
		}

		[Test]
		public void EnoughSilverToRareUpgradePositiveTest()
		{
			var baseClasses = new TestClassContainer[]
			{
				new TestClassContainer(new Skill[]{ }) { Name = ClassName.Mage },
				new TestClassContainer(new Skill[]{ }) { Name = ClassName.Warrior },
				new TestClassContainer(new Skill[]{ }) { Name = ClassName.Healer }
			};

			var currencyStorage = new CurrencyStorage();
			currencyStorage.ApplySave(new ICurrencySave[] { new TestCurrencySave { Type = CurrencyType.Silver, Value = 100 } });
			var characterClassStorage = new CharacterClassStorage(baseClasses);
			var save = new TestCharacterClassStorageSave()
			{
				ClassesSave = new List<ICharacterClassSave>()
				{
					new TestCharacterClassSave() { Name = ClassName.Warrior, Copies = 0, Experience = 0, Rare = Rare.Common }
				},
				ActiveClassesSave = new ClassName[0]
			};
			characterClassStorage.ApplySave(save);
			SoftToUpgradeRareCalculator.Initialize(new[] { 100 });
			var upgrader = new CharacterClassUpgrader(currencyStorage, characterClassStorage);
			Assert.AreEqual(true, upgrader.EnoughSilverToRareUpgrade(ClassName.Warrior));
		}

		[Test]
		public void EnoughSilverToLevelUpNegativeTest()
		{
			var baseClasses = new TestClassContainer[]
			{
				new TestClassContainer(new Skill[]{ }) { Name = ClassName.Mage },
				new TestClassContainer(new Skill[]{ }) { Name = ClassName.Warrior },
				new TestClassContainer(new Skill[]{ }) { Name = ClassName.Healer }
			};

			var currencyStorage = new CurrencyStorage();
			currencyStorage.ApplySave(new ICurrencySave[] { new TestCurrencySave { Type = CurrencyType.Silver, Value = 0 } });
			var characterClassStorage = new CharacterClassStorage(baseClasses);
			var save = new TestCharacterClassStorageSave()
			{
				ClassesSave = new List<ICharacterClassSave>()
				{
					new TestCharacterClassSave() { Name = ClassName.Warrior, Copies = 0, Experience = 0, Rare = Rare.Common }
				},
				ActiveClassesSave = new ClassName[0]
			};
			characterClassStorage.ApplySave(save);
			SoftToLevelUpCalculator.Initialize(new[] { 100 });

			var upgrader = new CharacterClassUpgrader(currencyStorage, characterClassStorage);
			Assert.AreEqual(false, upgrader.EnoughSilverToLevelUp(ClassName.Warrior));
		}

		[Test]
		public void EnoughSilverToLevelUpPositiveTest()
		{
			var baseClasses = new TestClassContainer[]
			{
				new TestClassContainer(new Skill[]{ }) { Name = ClassName.Mage },
				new TestClassContainer(new Skill[]{ }) { Name = ClassName.Warrior },
				new TestClassContainer(new Skill[]{ }) { Name = ClassName.Healer }
			};

			var currencyStorage = new CurrencyStorage();
			currencyStorage.ApplySave(new ICurrencySave[] { new TestCurrencySave { Type = CurrencyType.Silver, Value = 100 } });
			var characterClassStorage = new CharacterClassStorage(baseClasses);
			var save = new TestCharacterClassStorageSave()
			{
				ClassesSave = new List<ICharacterClassSave>()
				{
					new TestCharacterClassSave() { Name = ClassName.Warrior, Copies = 0, Experience = 0, Rare = Rare.Common }
				},
				ActiveClassesSave = new ClassName[0]
			};
			characterClassStorage.ApplySave(save);
			SoftToLevelUpCalculator.Initialize(new[] { 100 });

			var upgrader = new CharacterClassUpgrader(currencyStorage, characterClassStorage);
			Assert.AreEqual(true, upgrader.EnoughSilverToLevelUp(ClassName.Warrior));
		}

		[Test]
		public void RareUpgradeNotEnoughSilverTest()
		{
			var baseClasses = new TestClassContainer[]
			{ new TestClassContainer(new Skill[]{ }) { Name = ClassName.Warrior } };

			var currencyStorage = new CurrencyStorage();
			currencyStorage.ApplySave(new ICurrencySave[] { new TestCurrencySave { Type = CurrencyType.Silver, Value = 0 } });
			var characterClassStorage = new CharacterClassStorage(baseClasses);
			var save = new TestCharacterClassStorageSave()
			{
				ClassesSave = new List<ICharacterClassSave>()
				{ new TestCharacterClassSave() { Name = ClassName.Warrior, Copies = 100, Experience = 0, Rare = Rare.Common }},
				ActiveClassesSave = new ClassName[0]
			};
			characterClassStorage.ApplySave(save);
			SoftToUpgradeRareCalculator.Initialize(new[] { 100 });
			CopiesToUpgradeRareCalculator.Initialize(new[] { 1 });

			var upgrader = new CharacterClassUpgrader(currencyStorage, characterClassStorage);
			Assert.Throws<InvalidOperationException>(() => upgrader.UpgradeRare(ClassName.Warrior));
		}

		[Test]
		public void RareUpgradeNotEnoughCopiesTest()
		{
			var baseClasses = new TestClassContainer[]
			{ new TestClassContainer(new Skill[]{ }) { Name = ClassName.Warrior } };

			var currencyStorage = new CurrencyStorage();
			currencyStorage.ApplySave(new ICurrencySave[] { new TestCurrencySave { Type = CurrencyType.Silver, Value = 100 } });
			var characterClassStorage = new CharacterClassStorage(baseClasses);
			var save = new TestCharacterClassStorageSave()
			{
				ClassesSave = new List<ICharacterClassSave>()
				{ new TestCharacterClassSave() { Name = ClassName.Warrior, Copies = 3, Experience = 0, Rare = Rare.Common }},
				ActiveClassesSave = new ClassName[0]
			};
			characterClassStorage.ApplySave(save);
			SoftToUpgradeRareCalculator.Initialize(new[] { 100 });
			CopiesToUpgradeRareCalculator.Initialize(new[] { 5 });

			var upgrader = new CharacterClassUpgrader(currencyStorage, characterClassStorage);
			Assert.Throws<InvalidOperationException>(() => upgrader.UpgradeRare(ClassName.Warrior));
		}

		[Test]
		public void RareUpgradePositiveTest()
		{
			var baseClasses = new TestClassContainer[]
			{ new TestClassContainer(new Skill[]{ }) { Name = ClassName.Warrior } };

			var currencyStorage = new CurrencyStorage();
			currencyStorage.ApplySave(new ICurrencySave[] { new TestCurrencySave { Type = CurrencyType.Silver, Value = 100 } });
			var characterClassStorage = new CharacterClassStorage(baseClasses);
			var save = new TestCharacterClassStorageSave()
			{
				ClassesSave = new List<ICharacterClassSave>()
				{ new TestCharacterClassSave() { Name = ClassName.Warrior, Copies = 7, Experience = 0, Rare = Rare.Common }},
				ActiveClassesSave = new ClassName[0]
			};
			characterClassStorage.ApplySave(save);
			SoftToUpgradeRareCalculator.Initialize(new[] { 100 });
			CopiesToUpgradeRareCalculator.Initialize(new[] { 5 });

			var upgrader = new CharacterClassUpgrader(currencyStorage, characterClassStorage);
			upgrader.UpgradeRare(ClassName.Warrior);
			var warrior = characterClassStorage.OpenedClasses.First(x => x.Name == ClassName.Warrior);

			Assert.AreEqual(Rare.Rare, warrior.Rare);
			Assert.AreEqual(2, warrior.Copies);
			Assert.AreEqual(0, currencyStorage.Get(CurrencyType.Silver).Value);
		}

		[Test]
		public void LevelUpNotEnoughSilverTest()
		{
			var baseClasses = new TestClassContainer[]
			{ new TestClassContainer(new Skill[]{ }) { Name = ClassName.Warrior } };

			var currencyStorage = new CurrencyStorage();
			currencyStorage.ApplySave(new ICurrencySave[] { new TestCurrencySave { Type = CurrencyType.Silver, Value = 0 } });
			var characterClassStorage = new CharacterClassStorage(baseClasses);
			var save = new TestCharacterClassStorageSave()
			{
				ClassesSave = new List<ICharacterClassSave>()
				{ new TestCharacterClassSave() { Name = ClassName.Warrior, Copies = 100, Experience = 1, Rare = Rare.Common }},
				ActiveClassesSave = new ClassName[0]
			};
			characterClassStorage.ApplySave(save);
			SoftToLevelUpCalculator.Initialize(new[] { 100 });
			ExperienceToLevelUpCalculator.Initialize(new[] { 1 });

			var upgrader = new CharacterClassUpgrader(currencyStorage, characterClassStorage);
			Assert.Throws<InvalidOperationException>(() => upgrader.UpLevel(ClassName.Warrior));
		}

		[Test]
		public void LevelUpNotEnoughExperienceTest()
		{
			var baseClasses = new TestClassContainer[]
			{ new TestClassContainer(new Skill[]{ }) { Name = ClassName.Warrior } };

			var currencyStorage = new CurrencyStorage();
			currencyStorage.ApplySave(new ICurrencySave[] { new TestCurrencySave { Type = CurrencyType.Silver, Value = 0 } });
			var characterClassStorage = new CharacterClassStorage(baseClasses);
			var save = new TestCharacterClassStorageSave()
			{
				ClassesSave = new List<ICharacterClassSave>()
				{ new TestCharacterClassSave() { Name = ClassName.Warrior, Copies = 100, Experience = 0, Rare = Rare.Common }},
				ActiveClassesSave = new ClassName[0]
			};
			characterClassStorage.ApplySave(save);
			SoftToLevelUpCalculator.Initialize(new[] { 100 });
			ExperienceToLevelUpCalculator.Initialize(new[] { 1 });

			var upgrader = new CharacterClassUpgrader(currencyStorage, characterClassStorage);
			Assert.Throws<InvalidOperationException>(() => upgrader.UpLevel(ClassName.Warrior));
		}

		[Test]
		public void LevelUpPosisiveTest()
		{
			var baseClasses = new TestClassContainer[]
			{ new TestClassContainer(new Skill[]{ }) { Name = ClassName.Warrior } };

			var currencyStorage = new CurrencyStorage();
			currencyStorage.ApplySave(new ICurrencySave[] { new TestCurrencySave { Type = CurrencyType.Silver, Value = 100 } });
			var characterClassStorage = new CharacterClassStorage(baseClasses);
			var save = new TestCharacterClassStorageSave()
			{
				ClassesSave = new List<ICharacterClassSave>()
				{ new TestCharacterClassSave() { Name = ClassName.Warrior, Copies = 100, Experience = 1, Rare = Rare.Common }},
				ActiveClassesSave = new ClassName[0]
			};
			characterClassStorage.ApplySave(save);
			SoftToLevelUpCalculator.Initialize(new[] { 100 });
			ExperienceToLevelUpCalculator.Initialize(new[] { 1 });

			var upgrader = new CharacterClassUpgrader(currencyStorage, characterClassStorage);
			upgrader.UpLevel(ClassName.Warrior);

			var warrior = characterClassStorage.OpenedClasses.First(x => x.Name == ClassName.Warrior);
			Assert.AreEqual(1, warrior.Level);
			Assert.AreEqual(0, currencyStorage.Get(CurrencyType.Silver).Value);
		}
	}
}
