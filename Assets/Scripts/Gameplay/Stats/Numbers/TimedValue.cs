using Bones.Gameplay.Stats.Units;
using Bones.Gameplay.Stats.Units.Operations;

namespace Bones.Gameplay.Stats.Numbers
{
	public readonly struct TimedValue : IMultiplySupportable<TimedValue, TimedValue>, IAddSupportable<Value, TimedValue>
	{
		public Value Interval { get; init; }
		public Value Amount { get; init; }
		
		public TimedValue Multiply(TimedValue right) =>
			new()
			{
				Interval = Interval,
				Amount = Amount * new Value(right.Amount / right.Interval * Interval)
			};

		public TimedValue Add(Value right) =>
			new()
			{
				Interval = Interval,
				Amount = Amount + right
			};
	}
}