using System;

namespace Bones.Gameplay.Events.Callbacks
{
    public abstract class BaseCallback<T> : IObserver<T>
    {
        public void OnCompleted() { }

        public void OnError(Exception error)
        {
            throw error;
        }

        public abstract void OnNext(T value);
    }
}