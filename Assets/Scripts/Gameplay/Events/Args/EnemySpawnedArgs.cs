using Bones.Gameplay.Entities;
using UnityEngine;

namespace Bones.Gameplay.Events.Args
{
    public readonly struct EnemySpawnedArgs : IMessageArgs
    {
        public Vector2 Position { get; init; }
        public Enemy Subject { get; init; }
    }
}