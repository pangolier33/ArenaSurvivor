using Bones.Gameplay.LevelLoader;
using Bones.Gameplay.Meta;
using Saving;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Bones.UI
{
	public class Main : Screen
	{
		[SerializeField] private Button playButton;
		[SerializeField] private Button levelSelectButton;
		[Inject] private ILevelLoader _levelLoader;
		[Inject] private Level _level;

		public override void Initialize()
		{
			base.Initialize();

			playButton.onClick.RemoveListener(Play);
			playButton.onClick.AddListener(Play);
			levelSelectButton.onClick.RemoveListener(ShowSelectLevel);
			levelSelectButton.onClick.AddListener(ShowSelectLevel);
		}

		private void Play() => _levelLoader.Load(_level.Current.Value);

		private void ShowSelectLevel()
		{
			var strap = GetComponentInParent<MenuBootstrapper>();
			strap.Show<LevelSelect>();
		}
	}
}