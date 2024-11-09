using System;
using Bones.Gameplay.Experience;
using Zenject;

namespace Bones.UI.Presenters
{
    public class LevelPresenter : InjectedPresenter<int>
    {
        private IExperienceBank _bank;
    
        protected override IObservable<int> RetrieveModel()
        {
            return _bank.Level;
        }

        [Inject]    
        private void Inject(IExperienceBank bank)
        {
            _bank = bank;
        }
    }
}
