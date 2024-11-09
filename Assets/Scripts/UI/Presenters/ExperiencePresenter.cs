using System.Collections.Generic;
using Bones.Gameplay.Experience;
using Bones.UI.Bindings.Base;
using UniRx;
using UnityEngine;
using Zenject;

namespace Bones.UI.Presenters
{
    public class ExperiencePresenter : PrefabContainerPresenter, IInitializable
    {
        [SerializeReference] private IBinding _starsLimitBinding;
        
        private readonly Dictionary<IStarModel, PrefabPresenter> _presentersByModel = new();
        private IExperienceBank _bank;
        private BankSwitcher _bankSwitcher;

        public void Initialize()
        {
            _bank.Requested.Subscribe(OnRequested);
            _bank.Released.Subscribe(OnReleased);
            Bind(_bankSwitcher.Limit, _starsLimitBinding);
        }

        private void OnRequested(IStarModel model)
        {
            _presentersByModel.Add(model, Spawn(model.Completeness));
        }

        private void OnReleased(IStarModel model)
        {
            if (_presentersByModel.TryGetValue(model, out var presenter))
                Destroy(presenter.gameObject);
        }
        
        [Inject]
        private void Inject(IExperienceBank bank, BankSwitcher bankSwitcher)
        {
            _bank = bank;
            _bankSwitcher = bankSwitcher;
        }
    }
}