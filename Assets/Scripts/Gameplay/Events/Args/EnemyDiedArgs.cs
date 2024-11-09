using Bones.Gameplay.Entities;
using UnityEngine;

namespace Bones.Gameplay.Events.Args
{
    public readonly struct EnemyDiedArgs : IPositionalMessageArgs
    {
        public Enemy Subject { get; init; }
        public Vector2 Position { get; init; }
    }
}