using Bones.Gameplay.Effects.Provider;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
	public abstract class AddTEC<T> : TransitiveEffect
	{
		protected sealed override void Invoke(IMutableTrace trace, IEffect next)
		{
			next.Invoke(trace.Add(RetrieveArg(trace)));
		}

		protected abstract T RetrieveArg(ITrace trace);
	}
}