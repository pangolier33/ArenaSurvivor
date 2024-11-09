using UniRx;
using UnityEngine;

namespace Bones.Gameplay.Map
{
	public interface IPositionTracker
	{
		Vector2 Velocity { get; }
		IReadOnlyReactiveProperty<Vector2> Property { get; }
		IReadOnlyReactiveProperty<Vector2> ForcedProperty { get; }
	}
}