using Bones.Gameplay.Events.Callbacks.Templates.Base;
using Zenject;

namespace Bones.Gameplay.Spells.Classes
{
	public static class ContainerExtensions
	{
		public static void InjectWhenNeeded(this DiContainer container, object injectable)
		{
			if (injectable is IInjectable)
				container.Inject(injectable);
		}
	}
}