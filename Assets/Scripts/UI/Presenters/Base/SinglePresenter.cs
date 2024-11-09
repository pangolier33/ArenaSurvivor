using System;
using Bones.UI.Bindings.Base;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Bones.UI.Presenters
{
    public abstract class SinglePresenter : BasePresenter
    {
        [SerializeReference]
        [InlineProperty]
        [HideLabel]
        private IBinding _binding;

        protected void Subscribe<T>(IObservable<T> model) => Subscribe(model, _binding);
    }
}