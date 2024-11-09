#if UNITY_EDITOR
using Bones.Gameplay.Waves.Spawning.Amounts;
using Bones.Gameplay.Waves.Spawning.Positions;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace Bones.Gameplay.Waves
{
	public partial class SpawnerConfig
	{
		[UsedImplicitly]
		private string Name
		{
			[UsedImplicitly]
			get
			{
				string text = Enabled ? "\u2713" : "\u25a1";
				text += " ";
				text += _prefab == null ? "empty spawner" : _prefab.name;
				return text;
			}
		}
		
		private bool Validate_amountResolver(IAmountResolver amountResolver)
		{
			return amountResolver != null;
		}
		
		private bool Validate_spawningDelay(float spawningDelay)
		{
			return spawningDelay > 0;
		}
		
		private bool Validate_positionResolver(IPositionResolver positionResolver, ref string message)
		{
			switch (positionResolver)
			{
				case null:
				case InCirclePositionResolver:
				case RandomPositionResolverOutOfCameraScope:
					message = null;
					return true;
				default:
					message = $"{positionResolver.GetType().Name} is not supported";
					return false;
			}
		}
		
		private bool Validate_enemyOffsetResolver(IAmountFResolver amountResolver, ref string message)
		{
			message = default;
			return amountResolver is not null;
		}
		
		private void Toggle()
		{
			Enabled = !Enabled;
		}
		
		private string GetSuffix() => Enabled ? "\u2713" : "\u25a1";
		
		private void CreateSharedConfig()
		{
			if (!_sharedConfig)
			{
				_sharedConfig = ScriptableObject.CreateInstance<SpawnerSharedConfig>();
				string path = "Assets/ScriptableObjects/SpawnerSharedConfig.asset";
				path = AssetDatabase.GenerateUniqueAssetPath(path);
				AssetDatabase.CreateAsset(_sharedConfig, path);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
				EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<SpawnerSharedConfig>(path));
			}
		}
	}
}
#endif