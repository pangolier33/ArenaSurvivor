using Bones.Gameplay.LevelLoader;
using Bones.Gameplay.Meta;
using Saving;
using Sirenix.OdinInspector;
using TMPro;
using UI.Layout;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Views
{
	/// <summary>
	/// Display <see cref="LevelData"/> in UI 
	/// </summary>
	public class LevelView : MonoBehaviour
	{
		[SerializeField] private bool _showIndex = true;
		[SerializeField, Required] private RawImage _preview;
		[SerializeField, Required] private TMP_Text _label;
		[SerializeField, Required] private GameObject _lock;
		[ShowInInspector] private LevelData _data;
		[Inject] private Level _level;
		[Inject, ShowInInspector] private ILevelLoader _levelLoader;

		public void Fill(int index) => Fill(_levelLoader.levels[index]);

		public void Fill(LevelData data)
		{
			_data = data;
			if (_preview) _preview.texture = data.Preview.texture;
			if (_label)
			{
				if (_showIndex)
				{
					_label.text = $"{data.index}. {data.Name}";
				}
				else
				{
					_label.text = data.Name;
				}
			}
			_lock.SetActive(_levelLoader.levels.IndexOf(data) > _level.MaxUnlocked.Value);
			ResetLayout();
		}

		private void ResetLayout()
		{
			var fitter = _preview.GetComponent<RawImageAspectFitter>();
			if (fitter)
			{
				fitter.ResetLayout();
			}
		}

		public void Init(Level level, ILevelLoader levelLoader)
		{
			_level = level;
			_levelLoader = levelLoader;
		}
	}
}