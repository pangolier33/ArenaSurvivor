using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Plugins.UI.Effects
{
	public class TransformIndexToSprite : MonoBehaviour
	{
		[SerializeField] private Transform _root;
		[SerializeField] private Object _folder;

#if UNITY_EDITOR
		[Button]
		private void Execute()
		{
			if (!_folder) return;

			_root = _root ?? transform;
			int index = _root.GetSiblingIndex();
			var sprites = AssetDatabase.FindAssets("t:Sprite", new []{AssetDatabase.GetAssetPath(_folder)}).Select(AssetDatabase.GUIDToAssetPath).Select(AssetDatabase.LoadAssetAtPath<Sprite>).ToArray();
			if (sprites.Length > index)
			{
				GetComponent<Image>().sprite = sprites[index];
				EditorUtility.SetDirty(GetComponent<Image>());
			}
		}
#endif
	}
}