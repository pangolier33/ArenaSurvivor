using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

namespace Bones.Gameplay.Utils
{
    public static class TweenExtensions
    {
        public static TweenerCore<Color, Color, ColorOptions> TweenColoring(this Graphic graphic, Color color, float duration)
        {
            return DOTween.To(() => graphic.color, x => graphic.color = x, color, duration);
        }
        
        public static LazyTween<Color> TweenColoringLazy(this Graphic image)
        {
            return new LazyTween<Color>(image.TweenColoring);
        }
        
        public static TweenerCore<float, float, FloatOptions> TweenFilling(this Image image, float value, float duration)
        {
            return DOTween.To(() => image.fillAmount, x => image.fillAmount = x, value, duration);
        }

        public static LazyTween<float> TweenFillingLazy(this Image image)
        {
            return new LazyTween<float>(image.TweenFilling);
        }
        
        public static LazyTween<TValue> CreateLazy<TValue>(Func<TValue, float, Tweener> tweenBuilder)
        {
            return new LazyTween<TValue>(tweenBuilder);
        }
        
        public static IDisposable AsDisposable(this Tweener tween)
        {
            return new DisposableTween(tween);
        }
        
        public static IDisposable AsDisposable<T1, T2, T3>(this TweenerCore<T1, T2, T3> tween)
            where T3 : struct, IPlugOptions
        {
            return new DisposableTween(tween);
        }
        
        public class LazyTween<TValue> : IDisposable
        {
            private readonly Func<TValue, float, Tweener> _tweenBuilder;
            private Tweener _tween;
            
            public LazyTween(Func<TValue, float, Tweener> tweenBuilder)
            {
                _tweenBuilder = tweenBuilder;
            }

            public void Do(TValue value, float duration)
            {
                if (_tween == null)
                    _tween = _tweenBuilder.Invoke(value, duration);
                else
                    _tween.ChangeEndValue(value, duration, true);
                _tween.Play();
            }
            
            public void Dispose()
            {
                _tween?.Kill();
            }
        }
        
        private class DisposableTween : IDisposable
        {
            private readonly Tweener _tween;

            public DisposableTween(Tweener tween)
            {
                _tween = tween;
            }
            
            public void Dispose()
            {
                _tween.Kill();
            }
        }
    }
}