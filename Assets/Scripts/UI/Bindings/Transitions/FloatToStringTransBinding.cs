using System;
using Bones.UI.Bindings.Transitions.Base;
using UnityEngine;

namespace Bones.UI.Bindings.Transitions
{
	[Serializable]
	public class FloatToStringTransBinding : TransBinding<float, string>
	{
		[SerializeField] private string _format;
		
		protected override string Convert(float from)
		{
			return from.ToString(_format);
		}
	}
}