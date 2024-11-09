using System;
using Bones.UI.Bindings.Base;
using DG.Tweening;
using Railcar.Time;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Bones.UI.Bindings.Transitions.Base
{
    public abstract class TransBinding<TFrom, TTo> : IObserver<TFrom>, IBinding
    {
        [SerializeReference]
        [InlineProperty]
        [HideLabel]
        [PropertyOrder(float.MaxValue)]
        private IBinding _rawBinding;

        private IObserver<TTo> _observer;
        private IObserver<TTo> Observer => _observer ??= (IObserver<TTo>)_rawBinding;
        
        public void OnCompleted() => Observer.OnCompleted();
        public void OnError(Exception error) => Observer.OnError(error);
        public void OnNext(TFrom value) => Observer.OnNext(Convert(value));

        protected abstract TTo Convert(TFrom from);
    }
}