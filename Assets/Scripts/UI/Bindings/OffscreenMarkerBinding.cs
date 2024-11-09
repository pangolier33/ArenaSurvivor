using System;
using Bones.UI.Bindings.Base;
using UnityEngine;

namespace UI.Bindings
{
    [Serializable]
    public class OffscreenMarkerBinding : BaseBinding<Vector2>
    {
        [SerializeField] private RectTransform _subject;
        private float _offset;
        private Vector2 _size;
        private Camera _camera;

        public override void OnNext(Vector2 value)
        {
            if (!_camera)
            {
                _camera = Camera.main;
                _offset = Mathf.Max(_subject.rect.size.x, _subject.rect.size.y);
                _size = _subject.GetComponentInParent<Canvas>().GetComponent<RectTransform>().rect.size;
            }

            Vector2 viewportPosition = _camera.WorldToViewportPoint(value);
            bool offscreen = viewportPosition.x < 0 || viewportPosition.x > 1
                                                    || viewportPosition.y < 0 || viewportPosition.y > 1;
            if (offscreen)
            {
                _subject.gameObject.SetActive(true);
                var screenPosition = _camera.WorldToViewportPoint(value) * _size;
                screenPosition = new Vector2(
                    Mathf.Clamp(screenPosition.x, _offset, _size.x - _offset),
                    Mathf.Clamp(screenPosition.y, _offset, _size.y - _offset));
                var rotation = Quaternion.Euler(0, 0,
                    Vector2.SignedAngle(Vector2.up, viewportPosition - Vector2.one / 2f));
                _subject.position = screenPosition;
                _subject.rotation = rotation;
            }
            else
            {
                _subject.gameObject.SetActive(false);
            }
        }
    }
}