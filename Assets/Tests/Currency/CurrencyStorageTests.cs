using Bones.Gameplay.Meta.Currencies;
using NUnit.Framework;

namespace Assets.Tests
{
	public class CurrencyStorageTests
	{
		[Test]
		public void CreateTest()
		{
			var saves = new TestCurrencySave[]
			{
				new TestCurrencySave{Value = 100, Type = CurrencyType.Crystal},
				new TestCurrencySave{Value = 150, Type = CurrencyType.Silver}
			};
			ICurrencyStorage storage = new CurrencyStorage();
			storage.ApplySave(saves);
			Currency crystals = storage.Get(CurrencyType.Crystal);
			Assert.IsNotNull(crystals);
			Assert.AreEqual(CurrencyType.Crystal, crystals.Type);
			Assert.AreEqual(100, crystals.Value);
			Currency silver = storage.Get(CurrencyType.Silver);
			Assert.IsNotNull(silver);
			Assert.AreEqual(CurrencyType.Silver, silver.Type);
			Assert.AreEqual(150, silver.Value);
		}
	}
}
