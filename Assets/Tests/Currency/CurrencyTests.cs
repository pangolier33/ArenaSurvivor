using Bones.Gameplay.Meta.Currencies;
using NUnit.Framework;
using System;
using UnityEngine;

namespace Assets.Tests
{
	public class CurrencyTests : MonoBehaviour
	{

		[Test]
		public void TestCreate()
		{
			Currency currency = new Currency(0, CurrencyType.Silver);
			Assert.AreEqual(CurrencyType.Silver, currency.Type);
			Assert.AreEqual(0, currency.Value);
		}

		[Test]
		public void TestCreateDepositWithInitialValue()
		{
			Currency currency = new Currency(100, CurrencyType.Silver);
			Assert.AreEqual(100, currency.Value);
		}

		[Test]
		public void TestDeposit()
		{
			Currency currency = new Currency(0, CurrencyType.Silver);
			currency.Deposit(10);
			Assert.AreEqual(10, currency.Value);
		}

		[Test]
		public void TestTwoDeposit()
		{
			Currency currency = new Currency(0, CurrencyType.Silver);
			currency.Deposit(10);
			currency.Deposit(30);
			Assert.AreEqual(40, currency.Value);
		}

		[Test]
		public void TestIncorrectDeposit()
		{
			Currency currency = new Currency(0, CurrencyType.Silver);
			Assert.Throws<ArgumentOutOfRangeException>(() => currency.Deposit(-10));
		}

		[Test]
		public void TestWithdraw()
		{
			Currency currency = new Currency(50, CurrencyType.Silver);
			currency.Withdrow(25);
			Assert.AreEqual(25, currency.Value);
		}

		[Test]
		public void TestIncorrectWithdraw()
		{
			Currency currency = new Currency(50, CurrencyType.Silver);
			Assert.Throws<ArgumentOutOfRangeException>(() => currency.Withdrow(-25));
		}

		[Test]
		public void TestMoreThanAvailableWithdraw()
		{
			Currency currency = new Currency(50, CurrencyType.Silver);
			Assert.Throws<InvalidOperationException>(() => currency.Withdrow(100));
		}
	}
}
