using System.Linq;
using Bones.Gameplay.Utils;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;
using TweenExtensions = Bones.Gameplay.Utils.TweenExtensions;

namespace Bones.UI.Views
{
    public class RadialBarView : MonoBehaviour
    {
        private const float MaxFillAmount = 0.5f;

        [SerializeField] private ProceduralImage[] _maxImages;
        [SerializeField] private float _maxFillingDuration;
        [SerializeField] private ProceduralImage[] _valueImages;
        [SerializeField] private float _valueFillingDuration;
        [SerializeField] private ProceduralImage[] _delayedFillingImages;
        [SerializeField] private float _delayedFillingDuration;
        [SerializeField] private float _fillDelay;
        [SerializeField] private Gradient _valueGradient;
        [SerializeField] private Gradient _delayedGradient;
        [SerializeField] private float _valueColoringDuration;

        private float _maxValue;
        private float _value;
        private float _previousValue;

        private TweenExtensions.LazyTween<float>[] _valueFillingTweens;
        private TweenExtensions.LazyTween<Color>[] _valueColoringTweens;
        private TweenExtensions.LazyTween<float>[] _maxFillingTweens;
        private Sequence _delayTweenSequence;

        public void SetMax(float maxValue)
        {
            _maxValue = maxValue;
            RedrawMax();
            RedrawValue();
            RedrawDelayedImagesInstantly();
        }

        public void UpdateValue(float value)
        {
            _value = value;
            RedrawValue();
            RedrawDelayedValue();
        }

        private void RedrawMax()
        {
            var newValue = _maxValue * MaxFillAmount;
            foreach (var fillingTween in _maxFillingTweens)
                fillingTween.Do(newValue, _maxFillingDuration);
        }

        private void RedrawValue()
        {
            var newValue = _value * MaxFillAmount;
            var newColor = _valueGradient.Evaluate(_value / _maxValue);

            foreach (var fillingTween in _valueFillingTweens)
                fillingTween.Do(newValue, _valueFillingDuration);
            foreach (var coloringTween in _valueColoringTweens)
                coloringTween.Do(newColor, _valueColoringDuration);
        }

        private void RedrawDelayedValue()
        {
            var newValue = _value * MaxFillAmount;
            bool increase = newValue >= _previousValue;
            _previousValue = newValue;
            if (increase)
            {
                if (_delayTweenSequence?.IsComplete() ?? true)
                    RedrawDelayedImagesInstantly();
                return;
            }

            var newColor = _delayedGradient.Evaluate(_value / _maxValue);
            _delayTweenSequence?.Kill();
            _delayTweenSequence = DOTween.Sequence().AppendInterval(_fillDelay).AppendInterval(0);
            foreach (var delayedFillingImage in _delayedFillingImages)
            {
                _delayTweenSequence.Join(delayedFillingImage.DOFillAmount(newValue, _delayedFillingDuration));
                _delayTweenSequence.Join(delayedFillingImage.DOColor(newColor, _delayedFillingDuration));
            }
        }

        private void RedrawDelayedImagesInstantly()
        {
            foreach (var delayedFillingImage in _delayedFillingImages)
            {
                delayedFillingImage.fillAmount = _value * MaxFillAmount;
                delayedFillingImage.color = _delayedGradient.Evaluate(_value / _maxValue);
            }
        }

        private void Awake()
        {
            _valueFillingTweens = _valueImages.Select(x => x.TweenFillingLazy()).ToArray();
            _valueColoringTweens = _valueImages.Select(x => x.TweenColoringLazy()).ToArray();
            _maxFillingTweens = _maxImages.Select(x => x.TweenFillingLazy()).ToArray();
        }
    }
}