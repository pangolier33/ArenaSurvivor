using System;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
	[Serializable]
    public class FloatComparingTransitiveEffect : BinaryOperationTransitiveEffect<float, float, bool>
    {
        protected override bool Calculate(float arg1, float arg2)
        {
            return arg1 > arg2;
        }
    }
}