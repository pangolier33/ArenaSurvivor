using System;
using System.Collections.Generic;
using Bones.Gameplay.Flocks.Base;
using UnityEngine;

namespace Bones.Gameplay.Flocks.Pulling
{
    public class PullingContainer<TCluster, TEntity> : FlocksContainer<TCluster, TEntity>
        where TCluster : class, ICollection<TEntity>, new()
        where TEntity : class, IPullingEntity<TEntity>
    {
        private readonly Func<float> _minPullingDistance;

        protected PullingContainer(Vector2Int extents, float clusterSize, Transform target, IEnumerable<TEntity> entities, Func<float> minPullingDistance)
            : base(extents, clusterSize, target, entities)
        {
            _minPullingDistance = minPullingDistance;
        }

        protected override void OnUpdated(float deltaTime)
        {
            var targetPosition = TargetPosition;
            foreach (var entity in Entities)
            {
                var entityPosition = entity.Position;
                
                var gridPosition = GetPositionOnGrid(entityPosition);
                // releasing node, if the entity is out of grid's bounds
                if (!Grid.TryGet(gridPosition, out var currentCluster))
                {
                    entity.Dispose();
                    continue;
                }
                
                // handling distance and release if needed
                var delta = targetPosition - entityPosition;
                var distance = delta.magnitude;
                if (entity.IsDespawningDistance(distance))
                {
                    entity.Dispose();
                    continue;
                }

				if (entity.IsCollectingDistance(distance))
				{
					entity.Collect();
					continue;
				}
                
                // updating cluster's binding if entity shifted to another cluster
                var boundCluster = entity.Cluster;
                if (!currentCluster.Equals(boundCluster))
                {
                    boundCluster?.Remove(entity);
                    entity.Cluster = currentCluster;
                    currentCluster.Add(entity);
                }

                if (!entity.Moving && distance > _minPullingDistance())
                    continue;
                
                entity.Move(delta / distance, deltaTime);
            }
        }
    }
}