using Bones.Gameplay.Stats.Containers;
using Bones.Gameplay.Stats.Processors;
using Bones.Gameplay.Stats.Units.Operations;

namespace Bones.Gameplay
{
	public sealed class StatRoutingWrapper<T> : ISubtractStat<T>, IAddStat<T>, IGetStat<T>
		where T : IAddSupportable<T, T>, ISubtractSupportable<T, T>
	{
		ref IStatProcessor<T> ISubtractStat<T>.Processor => ref SubtractStat.Processor;
		ref IStatProcessor<T> IAddStat<T>.Processor => ref AddStat.Processor;
		ref IStatProcessor<T> IGetStat<T>.Processor => ref GetStat.Processor;
		
		public ISubtractStat<T> SubtractStat { get; init; }
		public IAddStat<T> AddStat { get; init; }
		public IGetStat<T> GetStat { get; init; }
		
		public void Subtract(T value) => SubtractStat.Subtract(value);
		public void Add(T value) => AddStat.Add(value);
		public T Get() => GetStat.Get();

		public override string ToString() => $"StatRoutingWrapper {{ Subtract = {SubtractStat}, Add = {AddStat}, Get = {GetStat} }}";
	}
}