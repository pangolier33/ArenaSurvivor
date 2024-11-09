using System;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
	[Serializable]
	public class IntComparingTransitiveEffect : BinaryOperationTransitiveEffect<int, int, bool>
	{
		protected override bool Calculate(int arg1, int arg2)
		{
			return arg1 > arg2;
		}
	}
}