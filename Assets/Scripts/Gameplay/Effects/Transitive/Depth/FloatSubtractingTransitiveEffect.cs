using System;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
	[Serializable]
	public class FloatSubtractingTransitiveEffect : BinaryOperationTransitiveEffect<float, float, float>
	{
		protected override float Calculate(float arg1, float arg2)
		{
			return arg1 - arg2;
		}
	}
}