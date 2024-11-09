using UnityEngine;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
	public interface IPositionSource
	{
		INewStats Stats { get; }
		Vector2 Position { get; }
	}
}