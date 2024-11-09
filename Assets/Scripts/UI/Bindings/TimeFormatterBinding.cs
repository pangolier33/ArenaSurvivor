using Bones.UI.Bindings.Base;
using System;
using Railcar.UI;

namespace Bones.UI.Bindings
{
	[Serializable]

	public class TimeFormatterBinding : PlainConvertingBinding<float, string>
	{
		protected override string Convert(float from) => $"{(int)from / 60:00}:{(int)from % 60:00}";
	}
}
