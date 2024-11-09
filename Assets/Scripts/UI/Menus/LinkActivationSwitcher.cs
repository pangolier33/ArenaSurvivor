using UnityEngine;

namespace Bones.UI.Menus
{
	public class LinkActivationSwitcher : MonoBehaviour
	{
		[SerializeField] private GameObject _antagonist;

		private void OnEnable()
		{
			_antagonist.gameObject.SetActive(false);
		}

		private void OnDisable()
		{
			_antagonist.gameObject.SetActive(true);
		}
	}
}