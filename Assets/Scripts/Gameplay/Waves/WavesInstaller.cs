using System.Linq;
using Bones.Gameplay.Entities.Stats;
using Bones.Gameplay.Waves.Spawning.Positions;
using Bones.Gameplay.Waves.Spawning.Positions.Origins;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Waves
{
    [CreateAssetMenu(menuName = "Waves/Compilation", fileName = "New WavesCompilation")]
    public class WavesInstaller : ScriptableObjectInstaller
    {
        [SerializeField]
        private ClampingStatsFactory.StatsKit _clampingStats;
        
        [SerializeField]
        private Vector2 _worldCenter;

        [SerializeField]
        private StatsBooster.Curves _boostingCurves;
    
        [SerializeField]
        [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "Name")]
        [InlineEditor(Expanded = false)]
        private WaveConfig[] _waves;

        public override void InstallBindings()
        {
            BindPositionResolver();
            BindStatsProcessor();
            
            Container.BindFactory<DraftSpawner.Settings, DraftSpawner, PlaceholderFactory<DraftSpawner.Settings, DraftSpawner>>();
            
            Container.Bind<SpawnerConfig.Adapter>()
                .AsSingle();
            
            Container.Bind<WaveConfig[]>()
                .FromInstance(_waves)
                .AsSingle();
            
            Container.BindInterfacesTo<Railcar.Dice.Dice>()
                .AsSingle();

        }

        private void BindPositionResolver()
        {
            Container.BindIFactory<PositionOriginName, IPositionResolver, IPositionResolver>()
                .FromFactory<PositionResolverFactory>()
                .WhenInjectedInto<SpawnerConfig.Adapter>();

            Container.Bind<WorldPositionResolver>()
                .WithArguments(_worldCenter)
                .WhenInjectedInto<PositionResolverFactory>();

            Container.Bind<LocalPositionResolver>()
                .AsSingle()
                .WhenInjectedInto<PositionResolverFactory>();
        }
        private void BindStatsProcessor()
        {
            // Booster
            var allWavesDuration = _waves.Where(x => x.Type != WaveType.Boss).Sum(x => x.Duration); 
            Container.Bind<float>()
                .FromInstance(allWavesDuration)
                .WhenInjectedInto<StatsBooster>();

            Container.Bind<StatsBooster.Curves>()
	            .FromInstance(_boostingCurves);

            Container.Bind<StatsBooster>().AsSingle().Lazy();
        }
    }
}