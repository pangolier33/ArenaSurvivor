using System;
using Bones.Gameplay.Effects.Transitive.Depth;
using Bones.Gameplay.Stats.Containers;
using Bones.Gameplay.Stats.Units.Operations;

namespace Bones.Gameplay
{
	public abstract class StatBuilderBase<T> : IStatBuilder where T : IMultiplySupportable<T, T>
	{
		IStat IStatBuilder.Build() => Build();
		IStat IStatBuilder.BuildWrapper(IStat wrappedStat) => BuildWrapper((IGetStat<T>)wrappedStat);
		public override string ToString() => throw new NotImplementedException();
		public abstract IStat BuildUncertain();
		public ReadOnlyStat<T> BuildReadOnly() => new(GetValue());
		public Stat<T> Build() => new(GetValue());
		public StatWrapper<T, T> BuildWrapper(IGetStat<T> stat) => new(stat, GetValue());
		protected abstract T GetValue();
	}
}