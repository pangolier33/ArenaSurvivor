using System;
using Bones.Gameplay.Stats.Containers;
using Bones.Gameplay.Stats.Processors;

namespace Bones.Gameplay.Stats.Nodes
{
	[Serializable]
	public class DelegateStat<T> : IGetStat<T>
	{
		private readonly Func<T> _getter;
		private IStatProcessor<T> _processor = DummyStatProcessor<T>.Instance;
		
		public DelegateStat(Func<T> getter) => _getter = getter;

		ref IStatProcessor<T> IGetStat<T>.Processor => ref _processor;
		public T Get() => _processor.Process(_getter.Invoke());
	}
}