using Railcar.Time;
using Railcar.Time.Mono;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Startup
{
    public class TimeInstaller : MonoInstaller
    {
        [SerializeField] private PausableTimeWrapper _timeLord;
        
        public override void InstallBindings()
        {
	        Container.Bind<ITimeLord>()
	                 .FromInstance(_timeLord);

	        BindFacade(_timeLord.FixedTime, TimeID.Fixed);
	        BindFacade(_timeLord.FrameTime, TimeID.Frame);
	        BindFacade(_timeLord.LateFrameTime, TimeID.LateFrame);
        }

        private void BindFacade(TimeFacade facade, TimeID id)
        {
	        Container.Bind<ITimer>().WithId(id).FromInstance(facade.Timer);
	        Container.Bind<IStopwatch>().WithId(id).FromInstance(facade.Stopwatch);
	        Container.Bind<IClock>().WithId(id).FromInstance(facade.Clock);
	        Container.Bind<IObservableClock>().WithId(id).FromInstance(facade.ObservableClock);
        }
    }
}