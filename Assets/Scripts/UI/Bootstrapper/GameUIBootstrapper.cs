using Bones.Gameplay.Inputs;
using Bones.Gameplay.Inputs.Joysticks;
using Railcar.Time;
using UnityEngine;
using Zenject;

namespace Bones.UI
{
    public class GameUIBootstrapper : UIBootstrapper, IInitializable
    {
        [SerializeField] private GameObject _windowPause;
        [SerializeField] private VariableJoystick _variableJoystick;
        
        private ITimeLord _timeLord;

		public override void InstallBindings()
        {
            base.InstallBindings();

            Container.BindInterfacesAndSelfTo<InputObservable>()
                .AsSingle()
                .NonLazy();
            
            Container.Bind<IDirectionalInput>()
                .To<InputNormalizer>()
                .AsSingle()
                .WithArguments(_variableJoystick)
                .WhenInjectedInto<InputObservable>();
        }

        public void Initialize()
        {
            _timeLord.DoesFlow = true;
        }
        
        public void OnPauseRequested()
        {
            if (!_timeLord.DoesFlow)
                return;
            _timeLord.DoesFlow = false;
            _windowPause.SetActive(true);
        }
        public void OnPauseReleasing()
        {
            _timeLord.DoesFlow = true;
            _windowPause.SetActive(false);
        }

        [Inject]
        private void Inject(ITimeLord timeLord)
        {
            _timeLord = timeLord;
        }
    }
}