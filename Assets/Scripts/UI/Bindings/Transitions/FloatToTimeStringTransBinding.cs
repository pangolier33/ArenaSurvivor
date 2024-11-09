using System;
using Bones.UI.Bindings.Transitions.Base;
using UnityEngine;

namespace Bones.UI.Bindings.Transitions
{
	[Serializable]
	public class FloatToTimeStringTransBinding : TransBinding<float, string>
	{
		protected override string Convert(float from) => $"{(int)from / 60:00}:{(int)from % 60:00}";
	}
}