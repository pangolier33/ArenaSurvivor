using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Bones.Gameplay.LevelLoader
{
	[CreateAssetMenu]
	public class LevelData : ScriptableObject
	{
		[field: SerializeField] public string Name { get; private set; }
		[field: SerializeField] public int index { get; private set; }
		[field: SerializeField, Required] public AssetReference SceneAsset { get; private set; }
		[field: SerializeField, Required] public Sprite Preview { get; private set; }
	}
}