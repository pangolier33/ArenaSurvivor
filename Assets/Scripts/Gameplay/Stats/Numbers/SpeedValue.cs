using System.Globalization;
using Bones.Gameplay.Stats.Units;
using Bones.Gameplay.Stats.Units.Operations;

namespace Bones.Gameplay.Players
{
	public readonly struct SpeedValue : IAddSupportable<Value, SpeedValue>
	{
		public bool IsBoosted { get; init; }
		public float Amplifier { get; init; }
		
		public SpeedValue Add(Value right) => new()
		{
			IsBoosted = IsBoosted,
			Amplifier = Amplifier + right
		};

		public override string ToString() => Amplifier.ToString(CultureInfo.CurrentCulture);
	}
}