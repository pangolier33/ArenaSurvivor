using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Bones.UI.Views
{
	public class ProgressView : MonoBehaviour
	{
		[SerializeField] private Image _value;
		[SerializeField] private TMP_Text _current;
		[SerializeField] private TMP_Text _total;

		public void Initialize(int current, int total)
		{
			_value.fillAmount = ((float)current) / total;
			_current.text = current.ToString();
			_total.text = current.ToString();
		}
	}
}