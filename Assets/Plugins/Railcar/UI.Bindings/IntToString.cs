using System;

namespace Railcar.UI.Bindings
{
	[Serializable]
	internal class IntToString : PlainConvertingBinding<int, string>
	{
		protected override string Convert(int source)
		{
			return source.ToString();
		}
	}
}