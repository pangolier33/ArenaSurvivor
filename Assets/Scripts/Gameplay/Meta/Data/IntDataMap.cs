using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;

namespace Bones.Gameplay.Meta
{
	[CreateAssetMenu(menuName = "Data/IntDataMap")]
	public class IntDataMap : ScriptableObject
	{
		private const string SEPARATOR = "\n";

		[SerializeField] private TextAsset _dataFile;

		[SerializeField, ListDrawerSettings(ShowIndexLabels = true, ShowPaging = true)]
		private int[] _dataMap;

		public int[] DataMap => _dataMap;

		[Button]
		private void ParseTextFileIfNeed()
		{
			if (_dataFile)
				_dataMap = _dataFile.text.Split(SEPARATOR).Select(int.Parse).ToArray();
		}
	}
}
