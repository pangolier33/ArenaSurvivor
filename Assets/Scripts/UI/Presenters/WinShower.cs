using Bones.Gameplay.Meta;
using UnityEngine;
using Zenject;

namespace Bones.UI
{
	public class WinShower : MonoBehaviour
	{
		[SerializeField] private WinView _winView;

		[Inject] private Level _level;

		private void Start()
		{
			_level.Completed += OnLevelCompleted;
		}

		private void OnLevelCompleted()
		{
			_winView.gameObject.SetActive(true);
		}
	}
}
