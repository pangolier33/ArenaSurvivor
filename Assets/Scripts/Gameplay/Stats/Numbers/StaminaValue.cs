using System.Globalization;
using Bones.Gameplay.Stats.Units;
using Bones.Gameplay.Stats.Containers;
using System;

namespace Bones.Gameplay.Players
{
	public readonly struct StaminaValue //: IAddSupportable<Value, Stamina>
	{
		public int maxScore { get; init; }
		public float regenVelocity { get; init; }		
		public float spendVelocity { get; init; }
		public float slowDownParameter { get; init; }

		public float minPartToUse { get; init; }

		public int enemyKillScores { get; init; }

		public void Add(UncertainStat<Value, Value, Value> currentStamina, Value right, Stat<bool> staminaZeroReached)
		{
			var max = new Value(maxScore);

			if (currentStamina.BaseValue >= max) return;				

			var clampedAmountInc = Math.Min(right, max - right);
			currentStamina.Add(new Value(clampedAmountInc));

			if (currentStamina.BaseValue >= minPartToUse * maxScore)
				staminaZeroReached.Set(false);
		}

		//public override string ToString() => Amplifier.ToString(CultureInfo.CurrentCulture);
	}
}