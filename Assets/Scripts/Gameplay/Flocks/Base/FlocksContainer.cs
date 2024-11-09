using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bones.Gameplay.Flocks.Base
{
	public abstract class FlocksContainer<TCluster, TEntity>
        where TCluster : class, ICollection<TEntity>, new()
    {
        private readonly float _clusterSize;
        private readonly Vector2Int _extents;
        
        private readonly Transform _target;
        private Vector2Int _center;

        protected FlocksContainer(Vector2Int extents, float clusterSize, Transform target, IEnumerable<TEntity> entities)
        {
            _extents = extents;
            Grid = new ClustersGrid<TCluster, TEntity>(extents * 2 + Vector2Int.one);
            _clusterSize = clusterSize;
            
            _target = target;
            Entities = entities;
            _center = GetRoundedPosition(TargetPosition);
        }

        public void Update(float deltaTime)
        {
            ShiftCenter();
            OnUpdated(deltaTime);
        }

        protected IEnumerable<TEntity> Entities { get; }
        protected ClustersGrid<TCluster, TEntity> Grid { get; }

        public IEnumerable<TEntity> OrderByDistance(Vector2 center, float distance, Func<TEntity, Vector2> positionRetriever)
        {
            var depth = Mathf.CeilToInt(distance / _clusterSize);
            var gridCenter = GetPositionOnGrid(center);
            foreach (var cluster in Grid.Traverse(gridCenter.x, gridCenter.y, depth))
            {
                foreach (var entity in cluster.OrderBy(x => (positionRetriever(x) - center).sqrMagnitude))
                {
                    yield return entity;
                }
            }
        }
        
        protected Vector2 TargetPosition => _target.position;
        
        protected abstract void OnUpdated(float deltaTime);
        
        protected Vector2Int GetPositionOnGrid(Vector2 worldPosition)
        {
            var roundedPosition = GetRoundedPosition(worldPosition);
            return roundedPosition - _center + _extents;
        }

        private Vector2Int GetRoundedPosition(Vector2 worldPosition)
        {
            var gridX = Mathf.RoundToInt(worldPosition.x / _clusterSize);
            var gridY = Mathf.RoundToInt(worldPosition.y / _clusterSize);
            
            return new Vector2Int(gridX, gridY);
        }
        
        private void ShiftCenter()
        {
            var center = GetRoundedPosition(TargetPosition);
            var delta = _center - center;
            
            if (delta.x == 0 && delta.y == 0)
                return;
            
            Grid.Shift(delta);
            _center = center;
        }
    }
}