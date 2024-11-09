using UnityEngine;

namespace Bones.Gameplay.Effects.Containers.Projectiles
{
	public class UntriggerProjectileActivator : MonoBehaviour
	{
		private ITriggerExitBridge _container;

		private void OnTriggerExit2D(Collider2D other)
		{
			_container.OnOut(other.gameObject);
		}

		private void Awake()
		{
			_container = GetComponentInParent<ITriggerExitBridge>();
		}
	}
}