using System.Collections.Generic;
using UnityEngine;

namespace Bones.Gameplay.Flocks.Boids
{
    public class BoidsCluster<T> : HashSet<T>
    {
        private Vector2? _centerOfMass;
        public Vector2 PositionsSum { get; private set; }
        public Vector2 VelocitiesSum { get; private set; }
        public Vector2 CenterOfMass => _centerOfMass ??= PositionsSum / Count;

        public void AddPhysicsInfo(Vector2 position, Vector2 velocity)
        {
            if (_centerOfMass != null)
            {
                PositionsSum = Vector2.zero;
                VelocitiesSum = Vector2.zero;
                _centerOfMass = null;
            }
            
            PositionsSum += position;
            VelocitiesSum += velocity;
        }
    }
}