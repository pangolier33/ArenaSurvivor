using Bones.Gameplay.LevelLoader;
using Saving;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Views
{
	/// <summary>
	/// Display <see cref="ClassData"/> in UI 
	/// </summary>
	public class ClassView : MonoBehaviour
	{
		[SerializeField] private bool _showIndex = true;
		[SerializeField, Required] private Image _preview;
		[SerializeField, Required] private TMP_Text _label;
		[SerializeField, Required] private GameObject _lock;
		[ShowInInspector] private ClassData _data;

		//public void Fill(int index) => Fill(_levelLoader.levels[index]);

		public void Fill(ClassData data)
		{
			_data = data;
			if (_preview) _preview.sprite = data.Preview;
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
		}

	}
}