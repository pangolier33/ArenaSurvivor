using UnityEngine;

namespace Bones.Gameplay.Events.Args
{
    public readonly struct EnemyDamagedArgs : IPositionalMessageArgs
    {
        public Vector2 Position { get; init; }
        public int Value { get; init; }
        public int RelativeValue { get; init; }
    }
}