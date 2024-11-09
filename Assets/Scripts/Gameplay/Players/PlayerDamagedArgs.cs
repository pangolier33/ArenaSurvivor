using Bones.Gameplay.Events.Args;
using UnityEngine;

namespace Bones.Gameplay.Players
{
	public readonly struct PlayerDamagedArgs : IPositionalMessageArgs
	{
		public Vector2 Position { get; init; }
	}
}