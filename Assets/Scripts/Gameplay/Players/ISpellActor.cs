using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Effects.Transitive.Depth;

namespace Bones.Gameplay.Players
{
	public interface ISpellActor
	{
		IStatMap Stats { get; }
		Pair Position { get; }
	}
}