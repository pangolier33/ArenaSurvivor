using System;
using System.Collections.Generic;
using UniRx;

namespace Bones.Gameplay.Experience
{
    public class BankSwitcher : IExperienceBank
    {
        private readonly Dictionary<BankType, IExperienceBank> _implementations;
        private readonly ReactiveProperty<int> _limit = new();
        private IExperienceBank _bankImplementation;
		
		public BankType LastBankType { get; private set; }
        
        public BankSwitcher(Dictionary<BankType, IExperienceBank> implementations)
        {
            _implementations = implementations;
        }

        public IObservable<int> Limit => _limit;

        public void Switch(BankType bankType)
        {
            _bankImplementation = _implementations[bankType];
            if (_bankImplementation is ReleasingExperienceBank releasingExperienceBank)
                _limit.Value = releasingExperienceBank.Limit;
			LastBankType = bankType;
        }

        void IExperienceBank.Obtain(float amount) => _bankImplementation.Obtain(amount);

        void IExperienceBank.ObtainPercent(float percent) => _bankImplementation.ObtainPercent(percent);

        IReadOnlyReactiveProperty<int> IExperienceBank.Level => _bankImplementation.Level;
        IObservable<IStarModel> IExperienceBank.Requested => _bankImplementation.Requested;
        IObservable<IStarModel> IExperienceBank.Released => _bankImplementation.Released;
    }
}