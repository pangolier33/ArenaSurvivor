using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Bones.Gameplay.Effects.Containers.Projectiles
{
	public class TimeProjectileDisconnector : MonoBehaviour
    {
        [SerializeField] private int _duration;

        private ProjectileEffectContainer _projectile;

        private async void OnEnable()
        {
            await UniTask.Delay(_duration);
            _projectile.Dispose();
        }

        private void Awake()
        {
            _projectile = GetComponentInParent<ProjectileEffectContainer>();
        }
    }
}