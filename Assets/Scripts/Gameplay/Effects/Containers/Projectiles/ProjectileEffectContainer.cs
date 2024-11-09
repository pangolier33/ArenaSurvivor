using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Effects.Provider.Branches;
using Bones.Gameplay.Entities;
using Bones.Gameplay.Factories.Pools.Mono;
using UnityEngine;

namespace Bones.Gameplay.Effects.Containers.Projectiles
{
	public class ProjectileEffectContainer : MonoPoolBridge<ProjectileSettings, ProjectileEffectContainer>, IProjectileHandle, ITriggerEnterBridge
    {
        #if UNITY_EDITOR
        private IMutableTrace _spawnedMutableTrace;
        private IMutableTrace _triggeredMutableTrace;
#else
        private IMutableTrace _trace;
        #endif

	    private IBranch _branch;
	    
        public ProjectileEffectContainer Instance => this;
        
        public void OnTriggered(GameObject enemyGO)
        {
            if (!IsSpawned)
                return;
			   
            if (enemyGO.TryGetComponent<Enemy>(out var enemy) && enemy.IsSpawned)
#if UNITY_EDITOR
				Settings.OnTriggeredEffect.Invoke(_triggeredMutableTrace.Add(enemy));
#else
				Settings.OnTriggeredEffect.Invoke(_trace.Add(enemy));
#endif
        }
        
        protected override void OnSpawned()
        {
            _branch = new Branch();
            _branch.Connect(this);
            Settings.Trace.ConnectToClosest(_branch);
            
            var provider = Settings.Trace.Add(_branch).Add(this);
#if UNITY_EDITOR
            _spawnedMutableTrace = provider.Add("OnSpawned");
            _triggeredMutableTrace = provider.Add("OnTriggered");
            Settings.OnSpawnedEffect.Invoke(_spawnedMutableTrace);
#else
            _trace = provider;
            Settings.OnSpawnedEffect.Invoke(_trace);
#endif
        }

        protected override void OnDespawned()
        {
#if UNITY_EDITOR
            _spawnedMutableTrace = null;

#else
			_trace = null;
#endif

	        _branch?.Dispose();
	        _branch = null;
        }
	}
}