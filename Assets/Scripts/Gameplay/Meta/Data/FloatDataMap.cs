using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;

namespace Bones.Gameplay.Meta
{
	[CreateAssetMenu(menuName = "Data/FloatDataMap")]
	public class FloatDataMap : ScriptableObject
	{
		private const string SEPARATOR = "\n";

		[SerializeField] private TextAsset _dataFile;

		[SerializeField, ListDrawerSettings(ShowIndexLabels = true, ShowPaging = true)]
		private float[] _dataMap;

		public float[] DataMap => _dataMap;

		[Button]
		private void ParseTextFileIfNeed()
		{
			if (_dataFile)
				_dataMap = _dataFile.text.Split(SEPARATOR).Select(float.Parse).ToArray();
		}
	}
}
