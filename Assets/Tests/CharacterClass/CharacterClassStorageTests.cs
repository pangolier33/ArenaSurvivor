using Bones.Gameplay.Meta.CharacterClases;
using Bones.Gameplay.Meta.Equipment;
using Bones.Gameplay.Spells.Classes;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Tests
{
	public class CharacterClassStorageTests
	{
		[Test]
		public void CreateTest()
		{
			var baseClasses = new TestClassContainer[]
			{
				new TestClassContainer(new Skill[]{ }) { Name = ClassName.Mage },
				new TestClassContainer(new Skill[]{ }) { Name = ClassName.Warrior },
				new TestClassContainer(new Skill[]{ }) { Name = ClassName.Healer }
			};
			var save = new TestCharacterClassStorageSave()
			{
				ClassesSave = new List<ICharacterClassSave>(),
				ActiveClassesSave = new List<ClassName>()
			};

			CharacterClassStorage storage = new(baseClasses);
			storage.ApplySave(save);
			
			Assert.AreEqual(3, storage.AllClasses.Count());
			Assert.AreEqual(0, storage.OpenedClasses.Count());
			Assert.AreEqual(0, storage.ActiveClasses.Count());
			Assert.AreEqual(1, storage.AllClasses.Count(x => x.Name == ClassName.Mage));
			Assert.AreEqual(1, storage.AllClasses.Count(x => x.Name == ClassName.Warrior));
			Assert.AreEqual(1, storage.AllClasses.Count(x => x.Name == ClassName.Healer));
		}

		[Test]
		public void CreateWithOpendSaveTest()
		{
			var baseClasses = new TestClassContainer[]
			{
				new TestClassContainer(new Skill[]{ }) { Name = ClassName.Mage },
				new TestClassContainer(new Skill[]{ }) { Name = ClassName.Warrior },
				new TestClassContainer(new Skill[]{ }) { Name = ClassName.Healer }
			};
			var save = new TestCharacterClassStorageSave()
			{
				ClassesSave = new List<ICharacterClassSave>()
				{
					new TestCharacterClassSave() { Name = ClassName.Mage, Copies = 10, Experience = 155, Rare = Rare.Draconic },
					new TestCharacterClassSave() { Name = ClassName.Healer, Copies = 5, Experience = 213, Rare = Rare.Magical }
				},
				ActiveClassesSave = new ClassName[0]
			};

			CharacterClassStorage storage = new(baseClasses);
			storage.ApplySave(save);

			Assert.AreEqual(3, storage.AllClasses.Count());
			Assert.AreEqual(2, storage.OpenedClasses.Count());
			Assert.AreEqual(0, storage.ActiveClasses.Count());
			var mage = storage.OpenedClasses.First(x => x.Name == ClassName.Mage);
			Assert.AreEqual(ClassName.Mage, mage.Name);
			Assert.AreEqual(10, mage.Copies);
			Assert.AreEqual(155, mage.Experience.Value);
			Assert.AreEqual(Rare.Draconic, mage.Rare);
			var healer = storage.OpenedClasses.First(x => x.Name == ClassName.Healer);
			Assert.AreEqual(ClassName.Healer, healer.Name);
			Assert.AreEqual(5, healer.Copies);
			Assert.AreEqual(213, healer.Experience.Value);
			Assert.AreEqual(Rare.Magical, healer.Rare);
		}

		[Test]
		public void CreateWithActiveSaveTest()
		{
			var baseClasses = new TestClassContainer[]
			{
				new TestClassContainer(new Skill[]{ }) { Name = ClassName.Mage },
				new TestClassContainer(new Skill[]{ }) { Name = ClassName.Warrior },
				new TestClassContainer(new Skill[]{ }) { Name = ClassName.Healer }
			};
			var save = new TestCharacterClassStorageSave()
			{
				ClassesSave = new List<ICharacterClassSave>()
				{
					new TestCharacterClassSave() { Name = ClassName.Mage, Copies = 10, Experience = 155, Rare = Rare.Draconic },
					new TestCharacterClassSave() { Name = ClassName.Healer, Copies = 5, Experience = 213, Rare = Rare.Magical }
				},
				ActiveClassesSave = new ClassName[] {ClassName.Mage, ClassName.Healer},
			};

			CharacterClassStorage storage = new(baseClasses);
			storage.ApplySave(save);

			Assert.AreEqual(3, storage.AllClasses.Count());
			Assert.AreEqual(0, storage.OpenedClasses.Count());
			Assert.AreEqual(2, storage.ActiveClasses.Count());
			var mage = storage.ActiveClasses.First(x => x.Name == ClassName.Mage);
			Assert.AreEqual(ClassName.Mage, mage.Name);
			Assert.AreEqual(10, mage.Copies);
			Assert.AreEqual(155, mage.Experience.Value);
			Assert.AreEqual(Rare.Draconic, mage.Rare);
			var healer = storage.ActiveClasses.First(x => x.Name == ClassName.Healer);
			Assert.AreEqual(ClassName.Healer, healer.Name);
			Assert.AreEqual(5, healer.Copies);
			Assert.AreEqual(213, healer.Experience.Value);
			Assert.AreEqual(Rare.Magical, healer.Rare);
		}

		[Test]
		public void OpenCharacterClassTest()
		{
			var baseClasses = new TestClassContainer[]
			{
				new TestClassContainer(new Skill[]{ }) { Name = ClassName.Mage },
				new TestClassContainer(new Skill[]{ }) { Name = ClassName.Warrior },
				new TestClassContainer(new Skill[]{ }) { Name = ClassName.Healer }
			};
			var save = new TestCharacterClassStorageSave()
			{
				ClassesSave = new List<ICharacterClassSave>(),
				ActiveClassesSave = new ClassName[0]
			};

			CharacterClassStorage storage = new(baseClasses);
			storage.ApplySave(save);
			storage.OpenClass(ClassName.Mage);

			Assert.AreEqual(3, storage.AllClasses.Count());
			Assert.AreEqual(1, storage.OpenedClasses.Count());
			Assert.AreEqual(0, storage.ActiveClasses.Count());

			var mage = storage.OpenedClasses.First(x => x.Name == ClassName.Mage);
		}

		[Test]
		public void ActivateCharacterClassTest()
		{
			var baseClasses = new TestClassContainer[]
			{
				new TestClassContainer(new Skill[]{ }) { Name = ClassName.Mage },
				new TestClassContainer(new Skill[]{ }) { Name = ClassName.Warrior },
				new TestClassContainer(new Skill[]{ }) { Name = ClassName.Healer }
			};
			var save = new TestCharacterClassStorageSave()
			{
				ClassesSave = new List<ICharacterClassSave>()
				{
					new TestCharacterClassSave() { Name = ClassName.Mage, Copies = 10, Experience = 155, Rare = Rare.Draconic }
				},
				ActiveClassesSave = new ClassName[0]
			};

			CharacterClassStorage storage = new(baseClasses);
			storage.ApplySave(save);
			storage.ActivateClass(ClassName.Mage);

			Assert.AreEqual(3, storage.AllClasses.Count());
			Assert.AreEqual(1, storage.OpenedClasses.Count());
			Assert.AreEqual(1, storage.ActiveClasses.Count());

			var mage = storage.ActiveClasses.First(x => x.Name == ClassName.Mage);
		}

		[Test]
		public void DeactivateCharacterClassTest()
		{
			var baseClasses = new TestClassContainer[]
			{
				new TestClassContainer(new Skill[]{ }) { Name = ClassName.Mage },
				new TestClassContainer(new Skill[]{ }) { Name = ClassName.Warrior },
				new TestClassContainer(new Skill[]{ }) { Name = ClassName.Healer }
			};
			var save = new TestCharacterClassStorageSave()
			{
				ClassesSave = new List<ICharacterClassSave>()
				{
					new TestCharacterClassSave() { Name = ClassName.Mage, Copies = 10, Experience = 155, Rare = Rare.Draconic }
				},
				ActiveClassesSave = new ClassName[] { ClassName.Mage }
			};

			CharacterClassStorage storage = new(baseClasses);
			storage.ApplySave(save);
			storage.DeactivateClass(ClassName.Mage);

			Assert.AreEqual(3, storage.AllClasses.Count());
			Assert.AreEqual(1, storage.OpenedClasses.Count());
			Assert.AreEqual(0, storage.ActiveClasses.Count());
		}

	}
}
