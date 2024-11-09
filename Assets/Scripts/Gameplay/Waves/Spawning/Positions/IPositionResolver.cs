using UnityEngine;

namespace Bones.Gameplay.Waves.Spawning.Positions
{
    public interface IPositionResolver
    {
        Vector2 PickUp();
    }
}