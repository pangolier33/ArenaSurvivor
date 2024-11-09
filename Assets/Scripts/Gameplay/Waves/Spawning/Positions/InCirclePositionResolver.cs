using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Bones.Gameplay.Waves.Spawning.Positions
{
    [Serializable]
    public class InCirclePositionResolver : IPositionResolver
    {
        [SerializeField] private Vector2 _range;

        public Vector2 PickUp()
        {
            var radius = Random.Range(_range.x, _range.y);
            return Random.insideUnitCircle.normalized * radius;
        }
    }
}