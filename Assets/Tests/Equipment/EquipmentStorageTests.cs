using Bones.Gameplay.Meta.Equipment;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Tests
{
	public class EquipmentStorageTests
	{
		[Test]
		public void CreateTest()
		{
			var save = new TestEquipmentStorageSave()
			{
				Backpack = new List<IEquipmentSave>(),
				Euiped = new List<IEquipmentSave>()
			};
			var equipmentData = new List<IEquipmentData>()
			{
				new TestEquipmentData() { Id = "111", Type = EquipmentType.Belt },
				new TestEquipmentData() { Id = "222", Type = EquipmentType.Boots },
				new TestEquipmentData() { Id = "333", Type = EquipmentType.Armor },
				new TestEquipmentData() { Id = "444", Type = EquipmentType.Glove }
			};
			var storage = new EquipmentStorage(equipmentData);
			storage.ApplySave(save);
			Assert.AreEqual(4, storage.AllEquipment.Count());
			var cape = storage.AllEquipment.First(x => x.Id == "111");
			Assert.AreEqual(EquipmentType.Belt, cape.Type);
			var boots = storage.AllEquipment.First(x => x.Id == "222");
			Assert.AreEqual(EquipmentType.Boots, boots.Type);
			var armor = storage.AllEquipment.First(x => x.Id == "333");
			Assert.AreEqual(EquipmentType.Armor, armor.Type);
			var glove = storage.AllEquipment.First(x => x.Id == "444");
			Assert.AreEqual(EquipmentType.Glove, glove.Type);
		}

		[Test]
		public void SaveBackpackTest()
		{
			var save = new TestEquipmentStorageSave()
			{
				Backpack = new List<IEquipmentSave>()
				{
					new TestEquipmentSave() { Id = "333", Rare = Rare.Magical},
					new TestEquipmentSave() { Id = "222", Rare = Rare.Legendary}
				},
				Euiped = new List<IEquipmentSave>()
			};
			var equipmentData = new List<IEquipmentData>()
			{
				new TestEquipmentData() { Id = "111", Type = EquipmentType.Belt },
				new TestEquipmentData() { Id = "222", Type = EquipmentType.Boots },
				new TestEquipmentData() { Id = "333", Type = EquipmentType.Armor },
				new TestEquipmentData() { Id = "444", Type = EquipmentType.Glove }
			};
			var storage = new EquipmentStorage(equipmentData);
			storage.ApplySave(save);

			Assert.AreEqual(2, storage.Backpack.Count());
			Assert.AreEqual(0, storage.Equiped.Count());
			var armor = storage.Backpack.First(x => x.Id == "333");
			Assert.AreEqual(Rare.Magical, armor.Rare);
			Assert.AreEqual(EquipmentType.Armor, armor.Type);
			var boots = storage.Backpack.First(x => x.Id == "222");
			Assert.AreEqual(Rare.Legendary, boots.Rare);
			Assert.AreEqual(EquipmentType.Boots, boots.Type);
		}

		[Test]
		public void SaveEquipedTest()
		{
			var save = new TestEquipmentStorageSave()
			{
				Backpack = new List<IEquipmentSave>(),
				Euiped = new List<IEquipmentSave>()
				{
					new TestEquipmentSave() { Id = "444", Rare = Rare.Mythical},
					new TestEquipmentSave() { Id = "111", Rare = Rare.Rare}
				}
			};
			var equipmentData = new List<IEquipmentData>()
			{
				new TestEquipmentData() { Id = "111", Type = EquipmentType.Belt },
				new TestEquipmentData() { Id = "222", Type = EquipmentType.Boots },
				new TestEquipmentData() { Id = "333", Type = EquipmentType.Armor },
				new TestEquipmentData() { Id = "444", Type = EquipmentType.Glove }
			};
			var storage = new EquipmentStorage(equipmentData);
			storage.ApplySave(save);

			Assert.AreEqual(0, storage.Backpack.Count());
			Assert.AreEqual(2, storage.Equiped.Count());
			var glove = storage.Equiped.First(x => x.Id == "444");
			Assert.AreEqual(Rare.Mythical, glove.Rare);
			Assert.AreEqual(EquipmentType.Glove, glove.Type);
			var cape = storage.Equiped.First(x => x.Id == "111");
			Assert.AreEqual(Rare.Rare, cape.Rare);
			Assert.AreEqual(EquipmentType.Belt, cape.Type);
		}

		[Test]
		public void EquipTest()
		{
			var save = new TestEquipmentStorageSave()
			{
				Backpack = new List<IEquipmentSave>()
				{
					new TestEquipmentSave() { Id = "333", Rare = Rare.Magical},
					new TestEquipmentSave() { Id = "222", Rare = Rare.Legendary}
				},
				Euiped = new List<IEquipmentSave>()
			};
			var equipmentData = new List<IEquipmentData>()
			{
				new TestEquipmentData() { Id = "111", Type = EquipmentType.Belt },
				new TestEquipmentData() { Id = "222", Type = EquipmentType.Boots },
				new TestEquipmentData() { Id = "333", Type = EquipmentType.Armor },
				new TestEquipmentData() { Id = "444", Type = EquipmentType.Glove }
			};

			var storage = new EquipmentStorage(equipmentData);
			storage.ApplySave(save);
			storage.Equip(storage.Backpack.First());

			Assert.AreEqual(1, storage.Backpack.Count());
			Assert.AreEqual(1, storage.Equiped.Count());
			var armor = storage.Equiped.First(x => x.Id == "333");
		}

		[Test]
		public void FailEquipTest()
		{
			var save = new TestEquipmentStorageSave()
			{
				Backpack = new List<IEquipmentSave>()
				{
					new TestEquipmentSave() { Id = "333", Rare = Rare.Magical},
					new TestEquipmentSave() { Id = "222", Rare = Rare.Legendary}
				},
				Euiped = new List<IEquipmentSave>()
			};
			var equipmentData = new List<IEquipmentData>()
			{
				new TestEquipmentData() { Id = "111", Type = EquipmentType.Belt },
				new TestEquipmentData() { Id = "222", Type = EquipmentType.Boots },
				new TestEquipmentData() { Id = "333", Type = EquipmentType.Armor },
				new TestEquipmentData() { Id = "444", Type = EquipmentType.Glove }
			};

			var storage = new EquipmentStorage(equipmentData);
			storage.ApplySave(save);

			Assert.Throws<InvalidOperationException>(() => storage.Equip(storage.AllEquipment.First()));
		}

		[Test]
		public void UnequipTest()
		{
			var save = new TestEquipmentStorageSave()
			{
				Backpack = new List<IEquipmentSave>()
				{
					new TestEquipmentSave() { Id = "333", Rare = Rare.Magical},
				},
				Euiped = new List<IEquipmentSave>()
				{
					new TestEquipmentSave() { Id = "222", Rare = Rare.Legendary}
				}
			};
			var equipmentData = new List<IEquipmentData>()
			{
				new TestEquipmentData() { Id = "111", Type = EquipmentType.Belt },
				new TestEquipmentData() { Id = "222", Type = EquipmentType.Boots },
				new TestEquipmentData() { Id = "333", Type = EquipmentType.Armor },
				new TestEquipmentData() { Id = "444", Type = EquipmentType.Glove }
			};

			var storage = new EquipmentStorage(equipmentData);
			storage.ApplySave(save);
			storage.Unequip(storage.Equiped.First());

			Assert.AreEqual(2, storage.Backpack.Count());
			Assert.AreEqual(0, storage.Equiped.Count());
		}

		[Test]
		public void FailUnequipTest()
		{
			var save = new TestEquipmentStorageSave()
			{
				Backpack = new List<IEquipmentSave>()
				{
					new TestEquipmentSave() { Id = "333", Rare = Rare.Magical},
				},
				Euiped = new List<IEquipmentSave>()
				{
					new TestEquipmentSave() { Id = "222", Rare = Rare.Legendary}
				}
			};
			var equipmentData = new List<IEquipmentData>()
			{
				new TestEquipmentData() { Id = "111", Type = EquipmentType.Belt },
				new TestEquipmentData() { Id = "222", Type = EquipmentType.Boots },
				new TestEquipmentData() { Id = "333", Type = EquipmentType.Armor },
				new TestEquipmentData() { Id = "444", Type = EquipmentType.Glove }
			};

			var storage = new EquipmentStorage(equipmentData);
			storage.ApplySave(save);

			Assert.Throws<InvalidOperationException>(() => storage.Unequip(storage.Backpack.First()));
		}

		[Test]
		public void AddToBackpackTest()
		{
			var save = new TestEquipmentStorageSave()
			{
				Backpack = new List<IEquipmentSave>(),
				Euiped = new List<IEquipmentSave>()
			};
			var equipmentData = new List<IEquipmentData>()
			{
				new TestEquipmentData() { Id = "111", Type = EquipmentType.Belt },
				new TestEquipmentData() { Id = "222", Type = EquipmentType.Boots },
				new TestEquipmentData() { Id = "333", Type = EquipmentType.Armor },
				new TestEquipmentData() { Id = "444", Type = EquipmentType.Glove }
			};

			var storage = new EquipmentStorage(equipmentData);
			storage.ApplySave(save);
			storage.AddToBackpack("222", Rare.Draconic, 2);
			storage.AddToBackpack("444", Rare.Magical, 3);

			Assert.AreEqual(2, storage.Backpack.Count());
			var boots = storage.Backpack.First(x => x.Id == "222");
			Assert.AreEqual(Rare.Draconic, boots.Rare);
			Assert.AreEqual(2, boots.RareLevel);
			var glove = storage.Backpack.First(x => x.Id == "444");
			Assert.AreEqual(Rare.Magical, glove.Rare);
			Assert.AreEqual(3, glove.RareLevel);
		}

		//[Test]
		//public void MergeTest()
		//{
		//	var save = new TestEquipmentStorageSave()
		//	{
		//		Backpack = new List<IEquipmentSave>()
		//		{
		//			new TestEquipmentSave() { Id = "333", Rare = Rare.Magical},
		//			new TestEquipmentSave() { Id = "333", Rare = Rare.Magical},
		//			new TestEquipmentSave() { Id = "333", Rare = Rare.Magical},
		//			new TestEquipmentSave() { Id = "222", Rare = Rare.Legendary}
		//		},
		//		Euiped = new List<IEquipmentSave>()
		//	};
		//	var equipmentData = new List<IEquipmentData>()
		//	{
		//		new TestEquipmentData() { Id = "111", Type = EquipmentType.Belt },
		//		new TestEquipmentData() { Id = "222", Type = EquipmentType.Boots },
		//		new TestEquipmentData() { Id = "333", Type = EquipmentType.Armor },
		//		new TestEquipmentData() { Id = "444", Type = EquipmentType.Glove }
		//	};

		//	var storage = new EquipmentStorage(equipmentData);
		//	storage.ApplySave(save);
		//	storage.Merge("333", Rare.Magical);

		//	Assert.AreEqual(2, storage.Backpack.Count());
		//	var armor = storage.Backpack.First(x => x.Id == "333");
		//	Assert.AreEqual(Rare.Mythical, armor.Rare);
		//}

		//[Test]
		//public void FailMergeTest()
		//{
		//	var save = new TestEquipmentStorageSave()
		//	{
		//		Backpack = new List<IEquipmentSave>()
		//		{
		//			new TestEquipmentSave() { Id = "333", Rare = Rare.Magical},
		//			new TestEquipmentSave() { Id = "333", Rare = Rare.Magical},
		//			new TestEquipmentSave() { Id = "222", Rare = Rare.Legendary}
		//		},
		//		Euiped = new List<IEquipmentSave>()
		//	};
		//	var equipmentData = new List<IEquipmentData>()
		//	{
		//		new TestEquipmentData() { Id = "111", Type = EquipmentType.Belt },
		//		new TestEquipmentData() { Id = "222", Type = EquipmentType.Boots },
		//		new TestEquipmentData() { Id = "333", Type = EquipmentType.Armor },
		//		new TestEquipmentData() { Id = "444", Type = EquipmentType.Glove }
		//	};

		//	var storage = new EquipmentStorage(equipmentData);
		//	storage.ApplySave(save);
		//	Assert.Throws<InvalidOperationException>(() => storage.Merge("333", Rare.Magical));
		//}

		//[Test]
		//public void FailRareMergeTest()
		//{
		//	var save = new TestEquipmentStorageSave()
		//	{
		//		Backpack = new List<IEquipmentSave>()
		//		{
		//			new TestEquipmentSave() { Id = "333", Rare = Rare.Magical},
		//			new TestEquipmentSave() { Id = "333", Rare = Rare.Magical},
		//			new TestEquipmentSave() { Id = "333", Rare = Rare.Magical},
		//			new TestEquipmentSave() { Id = "222", Rare = Rare.Legendary}
		//		},
		//		Euiped = new List<IEquipmentSave>()
		//	};
		//	var equipmentData = new List<IEquipmentData>()
		//	{
		//		new TestEquipmentData() { Id = "111", Type = EquipmentType.Belt },
		//		new TestEquipmentData() { Id = "222", Type = EquipmentType.Boots },
		//		new TestEquipmentData() { Id = "333", Type = EquipmentType.Armor },
		//		new TestEquipmentData() { Id = "444", Type = EquipmentType.Glove }
		//	};

		//	var storage = new EquipmentStorage(equipmentData);
		//	storage.ApplySave(save);

		//	Assert.Throws<InvalidOperationException>(() => storage.Merge("333", Rare.Common));
		//}
	}
}
