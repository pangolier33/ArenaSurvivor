using System;
using UniRx;

namespace Bones.Gameplay.Effects.Provider.Branches
{
    public sealed class Branch : IBranch
    {
        private readonly CompositeDisposable _subscriptions = new();
        private readonly string _id;
        
        public Branch(string id = null)
        {
            _id = id;
        }
        
        public void Connect(IDisposable subscription)
        {
            _subscriptions.Add(subscription);
        }
        public void Dispose()
        {
            _subscriptions.Clear();
        }
        public override string ToString()
        {
            return _id ?? "Unnamed branch";
        }
    }
}