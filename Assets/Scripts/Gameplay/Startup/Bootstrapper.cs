using System;
using Bones.Gameplay.Camera;
using Bones.Gameplay.Events.Callbacks.Templates.Base;
using Sirenix.OdinInspector;
using Zenject;

namespace Bones.Gameplay.Startup
{
	public class Bootstrapper : MonoInstaller, IInitializable
	{
		[ShowInInspector] private WavesHandler _wavesHandler;

		public override void InstallBindings()
		{
			_wavesHandler = new WavesHandler();
			Container.BindInterfacesTo<InputHandler>().AsSingle();
			Container.BindInterfacesTo<WavesHandler>().AsSingle();
			Container.BindInterfacesTo<ItemsHandler>().AsSingle();
			Container.BindInterfacesTo<LevelUpHandler>().AsSingle();
			Container.Bind<CameraFollower>().FromInstance(GetComponentInChildren<CameraFollower>());

			foreach (var initializable in GetComponentsInChildren<IInitializable>())
				Container.Bind<IInitializable>().FromInstance(initializable);
			foreach (var disposable in GetComponentsInChildren<IDisposable>())
				Container.Bind<IDisposable>().FromInstance(disposable);
			foreach (var injectable in GetComponentsInChildren<IInjectable>())
				Container.QueueForInject(injectable);
		}

		public void Initialize()
		{
			_wavesHandler = Container.Resolve<IWaveProvider>() as WavesHandler;
		}
	}
}