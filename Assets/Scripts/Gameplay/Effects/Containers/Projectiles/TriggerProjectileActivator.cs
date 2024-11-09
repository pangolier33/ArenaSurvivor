using UnityEngine;

namespace Bones.Gameplay.Effects.Containers.Projectiles
{
	public class TriggerProjectileActivator : MonoBehaviour
    {
        private ITriggerEnterBridge _projectile;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            _projectile.OnTriggered(other.gameObject);
        }

        private void Awake()
        {
            _projectile = GetComponentInParent<ITriggerEnterBridge>();
        }
    }
}