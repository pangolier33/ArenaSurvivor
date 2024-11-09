using System;
using Sirenix.OdinInspector;

namespace Bones.UI.Bindings.Base
{
    public abstract class BaseBinding<T> : IObserver<T>, IBinding
    {
        public virtual void OnCompleted() {}
        public void OnError(Exception error) => throw error;
        public abstract void OnNext(T value);
    }
}