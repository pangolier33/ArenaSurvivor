using Zenject;

namespace Bones.Gameplay.LoadOperations
{
    public class LoaderInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ILoadOperationExecutor>().To<LoadOperationExecutor>().AsSingle();
        }
    }
}