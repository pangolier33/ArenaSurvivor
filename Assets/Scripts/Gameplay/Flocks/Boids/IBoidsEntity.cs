using System;
using UnityEngine;

namespace Bones.Gameplay.Flocks.Boids
{
    public interface IBoidsEntity<T> : IDisposable
    {
        Vector2 Position { get; }
        Vector2 Velocity { get; }
        Vector2 Size { get; }
        BoidsCluster<T> Cluster { get; set; }
        bool HandleDistance(float distance);
        void Move(Vector2 direction);
    }
}