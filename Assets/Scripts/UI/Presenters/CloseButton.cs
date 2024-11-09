using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Presenters
{
	public class CloseButton : MonoBehaviour
	{
		[SerializeField] private Button _closeButton;
		[SerializeField] private Transform _closeObject;

		private void Start()
		{
			_closeButton.onClick.AddListener(() => _closeObject.gameObject.SetActive(false));
		}
	}
}