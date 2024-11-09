using Bones.Gameplay.Players;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Waves.Spawning.Positions.Origins
{
    public class LocalPositionResolver : IPositionResolver
    {
        private readonly Transform _target;

        private LocalPositionResolver([Inject] Player player)
        {
            _target = player.transform;
        }
        
        public Vector2 PickUp()
        {
            return _target.transform.position;
        }
    }
}