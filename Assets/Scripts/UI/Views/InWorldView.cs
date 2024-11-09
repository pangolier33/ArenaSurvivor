using Bones.Gameplay.Camera;
using UniRx;
using UnityEngine;
using Zenject;

namespace Bones.UI.Views
{
    public class InWorldView : MonoBehaviour, IInitializable
    {
        private CameraFollower _cameraFollower;
        
        private float _initialCameraSize;
        private Vector2 _initialScale;
        
        [Inject]
        private void Inject(CameraFollower cameraFollower)
        {
            _cameraFollower = cameraFollower;
        }
        
        public void Initialize()
        {
            _initialCameraSize = _cameraFollower.Size.Value;
            _initialScale = transform.localScale;
            _cameraFollower.Size.Subscribe(OnCameraSizeChanged);
        }
        
        private void OnCameraSizeChanged(float cameraSize)
        {
            transform.localScale = _initialScale * (_initialCameraSize / cameraSize);
        }
    }
}