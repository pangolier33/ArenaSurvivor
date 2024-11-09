using System;
using Bones.UI.Bindings.Base;
using TMPro;
using UnityEngine;

namespace UI.Bindings
{
	[Serializable]
	public class LabelFormatterBinding : BaseBinding<string>
	{
		[SerializeField] private TMP_Text _label;
		
		public override void OnNext(string value)
		{
			_label.text = string.Format(_label.text, value);
		}
	}
}