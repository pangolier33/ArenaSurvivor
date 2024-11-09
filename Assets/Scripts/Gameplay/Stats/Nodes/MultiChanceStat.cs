using System;
using Bones.Gameplay.Stats.Nodes;
using Bones.Gameplay.Stats.Processors;
using Bones.Gameplay.Stats.Units;
using Railcar.Dice;

namespace Bones.Gameplay.Stats.Containers
{
	public record MultiChanceStat : RecordStat, IGetStat<Amount>, ISetStat<Points>, IAddStat<Amount>, ISubtractStat<Amount>, IGetStat<Points>
	{
		private readonly IDice _dice;
		private Points _points;

		private IStatProcessor<Amount> _subtractProcessor = DummyStatProcessor<Amount>.Instance;
		private IStatProcessor<Amount> _addProcessor = DummyStatProcessor<Amount>.Instance;
		private IStatProcessor<Points> _getPointsProcessor = DummyStatProcessor<Points>.Instance;
		private IStatProcessor<Amount> _getAmountProcessor = DummyStatProcessor<Amount>.Instance;
		private IStatProcessor<Points> _setProcessor = DummyStatProcessor<Points>.Instance;
		
		public MultiChanceStat(Points points, IDice dice)
		{
			_points = points;
			_dice = dice;
		}

		ref IStatProcessor<Amount> ISubtractStat<Amount>.Processor => ref _subtractProcessor;
		ref IStatProcessor<Amount> IAddStat<Amount>.Processor => ref _addProcessor;
		ref IStatProcessor<Points> IGetStat<Points>.Processor => ref _getPointsProcessor;
		ref IStatProcessor<Amount> IGetStat<Amount>.Processor => ref _getAmountProcessor;
		ref IStatProcessor<Points> ISetStat<Points>.Processor => ref _setProcessor;

		public void Subtract(Amount value) => Set(_points - _subtractProcessor.Process(value));
		public void Add(Amount value) => Set(_points + _addProcessor.Process(value));
		Points IGetStat<Points>.Get() => _getPointsProcessor.Process(_points); 
		public Amount Get() => new(1 + ChanceToAmount(_dice.Get(), _getPointsProcessor.Process(_points).Percent));
		public void Set(Points value) => _points = _setProcessor.Process(value);
		private static int ChanceToAmount(double roll, double chance) => (int)(Math.Log(roll) / Math.Log(chance));

		public override string ToString() => $"MultiChance {{ BaseValue = {_points.ToString()}, GetValue = {_getPointsProcessor.Process(_points)} }}";
	}
}