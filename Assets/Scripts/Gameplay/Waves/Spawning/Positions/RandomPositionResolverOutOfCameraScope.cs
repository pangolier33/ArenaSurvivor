using System;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Bones.Gameplay.Waves.Spawning.Positions
{
    public sealed class RandomPositionResolverOutOfCameraScope : IPositionResolver
    {
        private readonly UnityEngine.Camera _camera;
        private readonly Transform _transform;

        public RandomPositionResolverOutOfCameraScope([NotNull] UnityEngine.Camera camera)
        {
            if (!camera.orthographic)
                throw new InvalidOperationException($"Camera must be orthographic to use with {nameof(RandomPositionResolverOutOfCameraScope)}");
            
            _camera = camera;
            _transform = camera.transform;
        }

        public Vector2 PickUp()
        {
            return GetRandomPositionOnSide() + (Vector2)_transform.position;
        }

        private Vector2 GetRandomPositionOnSide()
        {
            // TODO: extents
            var maxWidth = GetWidth();
            var maxHeight = GetHeight();

            return (isRight: Random.value > 0.5f, isTop: Random.value > 0.5f) switch
            {
                // right
                (true, true) => new Vector2(maxWidth, Random.Range(-maxHeight, maxHeight)),
                // left
                (false, true) => new Vector2(-maxWidth, Random.Range(-maxHeight, maxHeight)),
                // bottom
                (true, false) => new Vector2(Random.Range(-maxWidth, maxWidth), maxHeight),
                // top
                (false, false) => new Vector2(Random.Range(-maxWidth, maxWidth), -maxHeight),
            };
        }
        
        private float GetWidth()
        {
            return _camera.aspect * _camera.orthographicSize;
        }

        private float GetHeight()
        {
            return _camera.orthographicSize;
        }
    }
}