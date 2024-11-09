using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.LevelLoader
{
	public class LevelsInstaller : MonoInstaller
	{
		[SerializeField] private List<LevelData> _levels;

		public override void InstallBindings()
		{
			Container.Bind<ILevelLoader>().To<LevelLoader>().AsSingle().WithArguments(_levels);
		}

		[Button]
		private void Collect()
		{
#if UNITY_EDITOR
			_levels = AssetDatabase.FindAssets($"t:{nameof(LevelData)}").Select(AssetDatabase.GUIDToAssetPath).Select(AssetDatabase.LoadAssetAtPath<LevelData>).ToList();
  #endif
		}
	}
}