using Sirenix.OdinInspector.Editor;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Plugins.UI.Selectables
{
	public class SliderText : Slider
	{
		[SerializeField] private TMP_Text _text;
		[SerializeField] private string _format = "{0}/{1}";

		protected override void Set(float input, bool sendCallback = true)
		{
			base.Set(input, sendCallback);

			if (_text && !string.IsNullOrEmpty(_format))
			{
				_text.SetText(_format, value, maxValue, minValue);
			}
		}
	}
	
	#if UNITY_EDITOR
	[CustomEditor(typeof(SliderText))]
	public class SliderTextEditor : OdinEditor
	{
		
	}
	#endif
}