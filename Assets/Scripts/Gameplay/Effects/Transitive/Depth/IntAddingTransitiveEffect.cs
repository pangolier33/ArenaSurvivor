using System;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
    [Serializable]
    public class IntAddingTransitiveEffect : BinaryOperationTransitiveEffect<int, int, int>
    {
        protected override int Calculate(int arg1, int arg2)
        {
            return arg1 + arg2;
        }
    }
}