using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Plugins.Own.Animated
{
	[Serializable]
	public struct Vector3MinMax
	{
		[HorizontalGroup, LabelWidth(10)] public ParticleSystem.MinMaxCurve X;
		[HorizontalGroup, LabelWidth(10)] public ParticleSystem.MinMaxCurve Y;
		[HorizontalGroup, LabelWidth(10)] public ParticleSystem.MinMaxCurve Z;

		public Vector3 Evaluate(float t, float randomFactor = 0f, bool uniform = false)
		{
			if (uniform)
			{
				var value = X.Evaluate(t, randomFactor);
				return new Vector3(value, value, value);
			}
			return new Vector3(X.Evaluate(t, randomFactor), Y.Evaluate(t, randomFactor), Z.Evaluate(t, randomFactor));
		}
	}
}