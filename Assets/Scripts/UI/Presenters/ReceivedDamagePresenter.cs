using System;
using Bones.Gameplay.Players;
using Bones.Gameplay.Stats.Containers.Graphs;
using Bones.Gameplay.Stats.Units;
using Bones.Gameplay.Stats.Utils;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Presenters
{
    public class ReceivedDamagePresenter : MonoBehaviour, IInitializable
    {
        [Header("Background")] [SerializeField]
        private Image _background;
        [SerializeField] private Color _backgroundColor;
        [SerializeField] private float _backgroundColoringDuration;
        private Color _defaultColor;
        private Tween _colorTween;

        private IObservable<Value> _stats;


        [Inject]
        public void Inject(Player player)
        {
            _stats = player.Stats.Get(StatName.OverallHealth).ObserveOnSubtract<Value>();
        }

        private void OnDamaged(Value _)
        {
            _colorTween?.Kill();
            _colorTween = DOTween.Sequence()
                .Append(_background.DOColor(_backgroundColor, _backgroundColoringDuration))
                .Append(_background.DOColor(_defaultColor, _backgroundColoringDuration));
        }

        public void Initialize()
        {
            _defaultColor = _background.color;
            _stats.Subscribe(OnDamaged);
        }

        private void OnDestroy()
        {
            _colorTween?.Kill();
            _stats.Subscribe(OnDamaged);
        }
    }
}