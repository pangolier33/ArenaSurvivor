using Saving;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Bones.Gameplay.LevelLoader
{
	public class DontDestroy : MonoInstaller, IInitializable
	{
		[ShowInInspector] private Saver _saver;
		[ShowInInspector] private bool _isInitialized;

		public override void InstallBindings()
		{
			Container.BindInterfacesTo<DontDestroy>().FromInstance(this);
		}

		public void Initialize()
		{
			if (_isInitialized) return;
			_isInitialized = true;
			_saver = Resources.Load<Saver>("Saver");
			_saver.Load();
			if (SceneManager.GetActiveScene().buildIndex == (int)eScene.Splash)
			{
				SceneManager.LoadScene((int)eScene.Menu);
			}
		}
	}
}