using Zenject;

namespace Bones.UI
{
    public abstract class UIBootstrapper : MonoInstaller
    {
        public override void InstallBindings()
        {
            foreach (var viewModel in GetComponentsInChildren<IInitializable>())
                Container.Bind<IInitializable>().FromInstance(viewModel);
        }
    }
}