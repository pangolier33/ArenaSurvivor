using UnityEngine;

namespace Bones.Gameplay.Events.Args
{
    public interface IPositionalMessageArgs : IMessageArgs
    {
        Vector2 Position { get; }
    }
}