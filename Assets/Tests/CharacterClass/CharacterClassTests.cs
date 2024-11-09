using Bones.Gameplay.Meta.CharacterClases;
using Bones.Gameplay.Meta.Equipment;
using Bones.Gameplay.Spells.Classes;
using NUnit.Framework;
using System;
using System.Linq;

namespace Assets.Tests
{
	public class CharacterClassTests
	{
		[Test]
		public void CreateWithClassContainerTest()
		{
			IClassContainer classContainer = new TestClassContainer(new Skill[] { })
			{
				Name = ClassName.Healer
			};
			ExperienceToLevelUpCalculator.Initialize(new int[] {1});
			ICharacterClass characterClass = new CharacterClass(classContainer);
			Assert.AreEqual(ClassName.Healer, characterClass.Name);
			Assert.AreEqual(0, characterClass.Level);
			Assert.AreEqual(0, characterClass.Copies);
			Assert.AreEqual(0, characterClass.Skills.Count());
			Assert.AreEqual(Rare.Common, characterClass.Rare);
		}

		[Test]
		public void CreateWithOriginalTest()
		{
			IClassContainer classContainer = new TestClassContainer(new Skill[] { new Skill(null, ClassContainer.SpellType.Common) })
			{
				Name = ClassName.Warrior
			};
			ExperienceToLevelUpCalculator.Initialize(new int[] { 1 });
			ICharacterClass originalClass = new CharacterClass(classContainer);
			ICharacterClass copyClass = new CharacterClass(originalClass);
			Assert.AreEqual(ClassName.Warrior, copyClass.Name);
			Assert.AreEqual(0, copyClass.Level);
			Assert.AreEqual(0, copyClass.Copies);
			Assert.AreEqual(1, copyClass.Skills.Count());
			Assert.AreEqual(Rare.Common, copyClass.Rare);
		}

		[Test]
		public void CreateWithOriginalAndSaveTest()
		{
			var classContainer = new TestClassContainer(new Skill[] { null, null })
			{
				Name = ClassName.Mage
			};
			var originalClass = new CharacterClass(classContainer);
			var save = new TestCharacterClassSave()
			{
				Copies = 12,
				Experience = 560,
				Rare = Rare.Magical
			};
			ExperienceToLevelUpCalculator.Initialize(new int[] { 1 });
			ICharacterClass copyClass = new CharacterClass(originalClass, save);
			Assert.AreEqual(ClassName.Mage, copyClass.Name);
			Assert.AreEqual(0, copyClass.Level);
			Assert.AreEqual(12, copyClass.Copies);
			Assert.AreEqual(2, copyClass.Skills.Count());
			Assert.AreEqual(Rare.Magical, copyClass.Rare);
		}

		[Test]
		public void AddCopyTest()
		{
			var characterClass = new CharacterClass(new TestClassContainer(new Skill[] { }));
			Assert.AreEqual(0, characterClass.Copies);
			characterClass.AddCopy();
			Assert.AreEqual(1, characterClass.Copies);
		}

		[Test]
		public void FailUpgradeTest()
		{
			var characterClass = new CharacterClass(new TestClassContainer(new Skill[] { }));
			Assert.Throws<InvalidOperationException>(() => characterClass.UpgradeRare());
		}

		[Test]
		public void SuccessUpgradeTest()
		{
			var characterClass = new CharacterClass(new TestClassContainer(new Skill[] { }));
			Assert.AreEqual(Rare.Common, characterClass.Rare);
			CopiesToUpgradeRareCalculator.Initialize(new int[] { 1, 2, 3, 4, 5 });
			int copiesToUpgrade = CopiesToUpgradeRareCalculator.NeedToUpgradeRare(Rare.Common);
			for (int i = 0; i < copiesToUpgrade; i++)
				characterClass.AddCopy();
			Assert.AreEqual(copiesToUpgrade, characterClass.Copies);
			characterClass.UpgradeRare();
			Assert.AreEqual(0, characterClass.Copies);
			Assert.AreEqual(Rare.Rare, characterClass.Rare);
		}
	}
}
