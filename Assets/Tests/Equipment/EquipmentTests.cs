using Bones.Gameplay.Meta.Equipment;
using NUnit.Framework;
using UnityEngine;

namespace Assets.Tests
{
	public class EquipmentTests : MonoBehaviour
	{
		[Test]
		public void CreateTest()
		{
			var equipmentData = new TestEquipmentData()
			{
				Id = "111",
				Type = EquipmentType.Armor
			};
			Equipment equipment = new Equipment(equipmentData);
			Assert.AreEqual(Rare.Common, equipment.Rare);
			Assert.AreEqual(EquipmentType.Armor, equipment.Type);
			Assert.AreEqual("111", equipment.Id);
		}

		[Test]
		public void CreateFromCopyTest()
		{
			var equipmentData = new TestEquipmentData()
			{
				Id = "222",
				Type = EquipmentType.Boots
			};
			Equipment equipment = new Equipment(equipmentData);
			var copy = new Equipment(equipment, Rare.Magical, 3);
			Assert.AreEqual(Rare.Magical, copy.Rare);
			Assert.AreEqual(EquipmentType.Boots, copy.Type);
			Assert.AreEqual(3, copy.RareLevel);
			Assert.AreEqual("222", copy.Id);
		}

		[Test]
		public void IncreaseRareTest()
		{
			var equipmentData = new TestEquipmentData()
			{
				Id = "333",
				Type = EquipmentType.Glove
			};
			Equipment equipment = new Equipment(equipmentData);
			equipment.IncreaseRare();
			Assert.AreEqual(Rare.Common, equipment.Rare);
			Assert.AreEqual(2, equipment.RareLevel);
			equipment.IncreaseRare();
			Assert.AreEqual(Rare.Common, equipment.Rare);
			Assert.AreEqual(3, equipment.RareLevel);
			equipment.IncreaseRare();
			Assert.AreEqual(Rare.Rare, equipment.Rare);
			Assert.AreEqual(1, equipment.RareLevel);
		}
	}
}
