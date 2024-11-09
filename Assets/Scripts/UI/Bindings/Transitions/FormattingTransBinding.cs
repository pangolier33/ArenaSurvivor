using System;
using Bones.UI.Bindings.Transitions.Base;
using UnityEngine;

namespace Bones.UI.Bindings.Transitions
{
	[Serializable]
	public class FormattingTransBinding : TransBinding<string, string>
	{
		[SerializeField] private string _sample;
		
		protected override string Convert(string from)
		{
			return string.Format(_sample, from);
		}
	}
}