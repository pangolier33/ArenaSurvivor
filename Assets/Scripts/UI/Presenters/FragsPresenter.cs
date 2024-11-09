using System;
using Bones.Gameplay.Events.Args;
using Railcar;
using UniRx;
using Zenject;

namespace Bones.UI.Presenters
{
    public class FragsPresenter : InjectedPresenter<int>
    {
        private IMessageReceiver _receiver;

        protected override IObservable<int> RetrieveModel()
        {
            return _receiver.Receive<EnemyDiedArgs>()
                .AsCounter();
        }

        [Inject]
        private void Inject(IMessageReceiver receiver)
        {
            _receiver = receiver;
        }
    }
}