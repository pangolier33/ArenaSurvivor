using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bones.Gameplay.Flocks.Pulling
{
    public interface IPullingEntity<T> : IDisposable
    {
		bool Moving { get; }
        ICollection<T> Cluster { get; set; }
        Vector2 Position { get; }
        void Move(Vector2 force, float deltaTime);
		bool IsDespawningDistance(float distance);
		bool IsCollectingDistance(float distance);
		void Collect();
	}
}