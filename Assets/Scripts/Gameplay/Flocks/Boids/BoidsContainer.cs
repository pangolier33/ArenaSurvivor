using System.Collections.Generic;
using Bones.Gameplay.Flocks.Base;
using UnityEngine;

namespace Bones.Gameplay.Flocks.Boids
{
    public class BoidsContainer<T> : FlocksContainer<BoidsCluster<T>, T>
        where T : MonoBehaviour, IBoidsEntity<T>
    {
        private BoidsWeights _weights;

        public BoidsContainer(BoidsWeights weights, Vector2Int extents, float clusterSize, Transform target, IEnumerable<T> entities)
            : base(extents, clusterSize, target, entities)
        {
            _weights = weights;
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
                    if (entity.Cluster != null)
                    {
                        entity.Cluster.Remove(entity);
                        entity.Cluster = null;
                    }
                    
                    entity.Dispose();
                    continue;
                }
                
                // handling distance and release if needed
                var delta = targetPosition - entityPosition;
                if (entity.HandleDistance(delta.magnitude))
                {
                    if (entity.Cluster != null)
                    {
                        entity.Cluster.Remove(entity);
                        entity.Cluster = null;
                    }
                    
                    entity.Dispose();
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

                currentCluster.AddPhysicsInfo(entityPosition, entity.Velocity);
            }
            
            foreach (var entity in Entities)
            {
                var entityPosition = entity.Position;
                var cluster = entity.Cluster;

                if (cluster == null)
                {
                    Debug.Log(entity);
                    Debug.Log(entityPosition);
                }

                var targeting = targetPosition - entityPosition;
                targeting.Normalize();
                
                var cohesion = cluster.CenterOfMass - entityPosition;
                cohesion.Normalize();
                
                var separation = cluster.PositionsSum - entityPosition * cluster.Count;
                separation.Normalize();

                var emergencySeparation = Vector2.zero;
                var limitCounter = 0;
                var selfSize = entity.Size.magnitude;
                foreach (var neighbour in cluster)
                {
                    if (neighbour == entity)
                        continue;
                    if (limitCounter++ >= _weights.CountLimiter)
                        break;

                    var delta = entityPosition - (Vector2)neighbour.transform.position;
                    var distance = delta.magnitude;
                    if (distance < selfSize)
                    {
                        emergencySeparation = delta / distance;
                        break;
                    }
                }
                
                var alignment = cluster.VelocitiesSum;
                alignment.Normalize();

                var direction = targeting * _weights.Targeting +
                                cohesion * _weights.Cohesion +
                                separation * _weights.Separation +
                                emergencySeparation * _weights.EmergencySeparation +
                                alignment * _weights.Alignment;
                entity.Move(direction);
            }
        }
    }
}