using Bones.Gameplay.LevelLoader;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Bones.UI.Presenters
{
	public class LoadLevelPresenter : MonoBehaviour
	{
		private ILevelLoader _levelLoader;

		[Inject]
		private void Inject(ILevelLoader levelLoader)
		{
			_levelLoader = levelLoader;
			GetComponent<Button>().onClick.RemoveListener(Load);
			GetComponent<Button>().onClick.AddListener(Load);
		}

		public void Load()
		{
			_levelLoader.Load(0);
		}
	}
}