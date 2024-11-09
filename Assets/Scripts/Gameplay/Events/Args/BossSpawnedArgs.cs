using UnityEngine;

namespace Bones.Gameplay.Events.Args
{
	public readonly struct BossSpawnedArgs : IPositionalMessageArgs
	{
		public Vector2 Position { get; init; }
	}
}