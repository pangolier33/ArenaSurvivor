using Bones.Gameplay.Stats.Nodes;
using Bones.Gameplay.Stats.Processors;
using Bones.Gameplay.Stats.Units;

namespace Bones.Gameplay.Stats.Containers
{
	public record CooldownStat : RecordStat, IGetStat<Value>, IGetStat<Points>
	{
		private readonly Value _duration;
		private readonly Points _decrement;

		private IStatProcessor<Points> _pointsProcessor = DummyStatProcessor<Points>.Instance;
		private IStatProcessor<Value> _valueProcessor = DummyStatProcessor<Value>.Instance;

		public CooldownStat(Value duration, Points decrement)
		{
			_duration = duration;
			_decrement = decrement;
		}

		ref IStatProcessor<Points> IGetStat<Points>.Processor => ref _pointsProcessor;
		ref IStatProcessor<Value> IGetStat<Value>.Processor => ref _valueProcessor;

		Points IGetStat<Points>.Get() => GetDecrementPoints();
		Value IGetStat<Value>.Get() => GetCooldown();

		private Points GetDecrementPoints()
		{
			return _pointsProcessor.Process(_decrement);
		}

		private Value GetCooldown()
		{
			var decrementPercent = 1 - GetDecrementPoints().Percent;
			var unprocessedResult = new Value(_duration * decrementPercent);
			return _valueProcessor.Process(unprocessedResult);
		}
	}
}