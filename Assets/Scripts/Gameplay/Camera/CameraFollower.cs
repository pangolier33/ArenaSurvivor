using System;
using System.Linq;
using Bones.Gameplay.Events.Callbacks.Templates.Base;
using Bones.Gameplay.Startup;
using UniRx;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Camera
{
    public class CameraFollower : MonoBehaviour, IInitializable, IInjectable
    {
        [SerializeField] private UnityEngine.Camera _camera;
        [Header("Moving")] [SerializeField] private float _movingSmoothFactor;
        [SerializeField] private float _maxMovingSpeed = float.PositiveInfinity;
        [Header("Sizing")] [SerializeField] private AnimationCurve _sizeByVelocity;
        [SerializeField] private float _sizingSmoothFactor;
        [SerializeField] private float _maxSizingSpeed = float.PositiveInfinity;
        [SerializeField] private float _sizeIncreasingDelay;
        [SerializeField] private float _sizeDecreasingDelay;
        [SerializeField] private CameraWaveSizeConfig[] _sizeConfigs;

        private readonly ReactiveProperty<float> _size = new();
        private Transform _self;
        private Vector3 _movingVelocity;
        private float _sizingVelocity;
        private float _sizeIncreasingTime;
        private float _sizeDecreasingTime;
        private float _currentSizeOffset;
        private IWaveProvider _waveProvider;

        public IReadOnlyReactiveProperty<float> Size => _size;

        public void Follow(Vector3 targetPosition, float deltaTime)
        {
            var selfPos = _self.position;

            // builds target pos from self's z and x, y of the target
            var newPos = new Vector3(targetPosition.x, targetPosition.y, selfPos.z);
            // calculates current position as damped position,
            // and retrieves velocity to compute size
            var dampedPos = Vector3.SmoothDamp(
                selfPos,
                newPos,
                ref _movingVelocity,
                _movingSmoothFactor,
                _maxMovingSpeed,
                deltaTime
            );

            // applying            
            _self.position = dampedPos;

            // getting size from the curve
            var targetSize = _sizeByVelocity.Evaluate(_movingVelocity.magnitude) + _currentSizeOffset;
            var currentSize = _camera.orthographicSize;

            // if the size should be decreased
            if (targetSize < currentSize)
            {
                _sizeIncreasingTime = 0;
                if (_sizeDecreasingTime < _sizeDecreasingDelay)
                {
                    _sizeDecreasingTime += deltaTime;
                    return;
                }
            }
            // if the size should be increased
            else
            {
                _sizeDecreasingTime = 0;
                if (_sizeIncreasingTime < _sizeIncreasingDelay)
                {
                    _sizeIncreasingTime += deltaTime;
                    return;
                }
            }

            // calculates current velocity as damped
            var dampedSize = Mathf.SmoothDamp(
                currentSize,
                targetSize,
                ref _sizingVelocity,
                _sizingSmoothFactor,
                _maxSizingSpeed,
                deltaTime);

            // applying
            _camera.orthographicSize = dampedSize;
            _size.Value = _camera.orthographicSize;
        }

        [Inject]
        private void Inject(IWaveProvider waveProvider)
        {
            _waveProvider = waveProvider;
            _waveProvider.Index.Subscribe(OnWaveChanged);
        }

        private void OnWaveChanged(int index)
        {
            var sizeConfig = _sizeConfigs.FirstOrDefault(x => x.Wave == index);
            _currentSizeOffset = sizeConfig?.SizeOffset ?? _currentSizeOffset;
        }

        public virtual void Initialize()
        {
            if (!_camera.orthographic)
                throw new InvalidOperationException(
                    $"Camera must be orthographic to use with {nameof(CameraFollower)}");

            _size.Value = _camera.orthographicSize;
            _self = _camera.transform;
        }
    }
}