using Bones.Gameplay.LevelLoader;
using Bones.Gameplay.Meta;
using Saving;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UI.Navigation.Swipe;
using UI.Views;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Bones.UI
{
	public class LevelSelect : Screen
	{
		[SerializeField, Required] private Saver _saver;
		[SerializeField, Required] private Button backButton;
		[SerializeField, Required] private Button selectButton;
		[SerializeField, Required] private LevelView mainView;
		[SerializeField, Required] private LevelView prefab;
		private HorizontalSwipeGroup _swipeGroup;
		[Inject] private ILevelLoader _levelLoader;
		[Inject] private Level _level;
		private bool _isInitialized;

		public override void Initialize()
		{
			if (_isInitialized) return;
			base.Initialize();
			_isInitialized = true;
			CustomInitialize();
		}

		public void CustomInitialize()
		{
			backButton.onClick.RemoveListener(OnClickBack);
			backButton.onClick.AddListener(OnClickBack);
			selectButton.onClick.RemoveListener(OnClickSelect);
			selectButton.onClick.AddListener(OnClickSelect);
			_swipeGroup = GetComponentInChildren<HorizontalSwipeGroup>(true);
			mainView.Fill(_level.Current.Value);
			Clear();
			for (var i = 0; i < _levelLoader.levels.Count; i++)
			{
				var levelData = _levelLoader.levels[i];
				LevelView view = Instantiate(prefab, _swipeGroup.ScrollRect);
				view.Init(_level, _levelLoader);
				view.Fill(levelData);
				var events = view.GetComponent<SwipeGroupElementEvents>();
				int index = i;
				events.OnSelect.AddListener(() => { OnSelectLevel(index); });
			}
		}

		private void OnEnable()
		{
			if (!_isInitialized)
				return;

			_swipeGroup.Init(FindObjectOfType<MenuBootstrapper>());

			_swipeGroup.Select(_level.Current.Value, transform.root.GetComponent<MonoBehaviour>());

			_swipeGroup.GetComponentsInChildren<LevelView>().ForEach(view =>
			{
			});
		}

		private void Clear()
		{
			for (int i = _swipeGroup.ScrollRect.childCount - 1; i >= 0; i--)
			{
				Destroy(_swipeGroup.ScrollRect.GetChild(i).gameObject);
			}
			_swipeGroup.ScrollRect.DetachChildren();
		}

		private void OnSelectLevel(int index)
		{
			if (index <= _level.MaxUnlocked.Value)
				_level.SetCurrent(index);
		}

		private void OnClickSelect()
		{
			mainView.Fill(_levelLoader.levels[_level.Current.Value]);
			var strap = GetComponentInParent<MenuBootstrapper>();
			strap.Show<Main>();
		}

		private void OnClickBack()
		{
			var strap = GetComponentInParent<MenuBootstrapper>();
			strap.Show<Main>();
		}
	}
}