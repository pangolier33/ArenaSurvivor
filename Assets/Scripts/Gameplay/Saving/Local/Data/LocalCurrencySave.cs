using Bones.Gameplay.Meta.Currencies;
using System;

namespace Bones.Gameplay.Saving.Local
{
	[Serializable]
	public class LocalCurrencySave : ICurrencySave
	{
		public CurrencyType _typeLocal;
		public float _valueLocal;

		public CurrencyType Type
		{
			get { return _typeLocal;}
			set { _typeLocal = value; }
		}

		public float Value
		{
			get { return _valueLocal; }
			set { _valueLocal = value; }
		}
	}
}
