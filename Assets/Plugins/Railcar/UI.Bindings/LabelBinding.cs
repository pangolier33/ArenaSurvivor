using System;
using TMPro;
using UnityEngine;

namespace Railcar.UI.Bindings
{
	[Serializable]
	internal sealed class LabelBinding : BaseBinding<string>
	{
		[SerializeField] private TMP_Text _label;
		
		public override void OnNext(string value) => _label.text = value;
	}
}