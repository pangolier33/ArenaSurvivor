using Bones.Gameplay.Stats.Containers;
using Bones.Gameplay.Stats.Units.Operations;

namespace Bones.Gameplay
{
	public abstract class UncertainStatBuilderBase<TValue, TAdd, TSubtract> : StatBuilderBase<TValue>
		where TValue : IAddSupportable<TAdd, TValue>, ISubtractSupportable<TSubtract, TValue>, IMultiplySupportable<TValue, TValue>
	{
		public override IStat BuildUncertain() => BuildConcrete();
		public UncertainStat<TValue, TAdd, TSubtract> BuildConcrete() => new(GetValue());
	}
}