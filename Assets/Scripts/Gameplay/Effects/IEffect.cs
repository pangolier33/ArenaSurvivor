using Bones.Gameplay.Effects.Provider;

namespace Bones.Gameplay.Effects
{
	public interface IEffect
	{
		void Invoke(IMutableTrace trace);
	}
}