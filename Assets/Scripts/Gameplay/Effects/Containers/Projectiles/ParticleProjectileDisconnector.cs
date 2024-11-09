using UnityEngine;

namespace Bones.Gameplay.Effects.Containers.Projectiles
{
	public class ParticleProjectileDisconnector : MonoBehaviour
    {
        private ProjectileEffectContainer _projectile;

        private void OnParticleSystemStopped()
        {
            _projectile.Dispose();
        }

        private void Awake()
        {
            _projectile = GetComponentInParent<ProjectileEffectContainer>();
        }
    }
}