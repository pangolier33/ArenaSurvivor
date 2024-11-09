using System;
using Bones.UI.Bindings.Base;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace UI.Bindings
{
    public class ScaleAnimationBinding : BaseBinding<Action>
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _scale = 0.9f;
        [SerializeField] private float _duration = 0.1f;
        
        public override void OnNext(Action callback)
        {
            var seq = DOTween.Sequence(_target);
            seq.Append(_target.DOScale(Vector3.one * _scale, _duration));
            seq.Append(_target.DOScale(Vector3.one, _duration));
            seq.AppendCallback(callback.Invoke);
            seq.Play();
        }
    }
}