using UnityEngine;

namespace Bones.Gameplay.Waves.Spawning.Positions.Origins
{
    public sealed class WorldPositionResolver : IPositionResolver
    {
        private readonly Vector2 _center;
        
        public WorldPositionResolver(Vector2 center)
        {
            _center = center;
        }
        
        public Vector2 PickUp()
        {
            return _center;
        }
    }
}