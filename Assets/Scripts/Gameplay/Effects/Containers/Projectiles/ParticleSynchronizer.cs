using UnityEngine;

namespace Bones.Gameplay.Effects.Containers.Projectiles
{
	public abstract class ParticleSynchronizer : MonoBehaviour
	{
		[SerializeField] private ParticleSystem _system;
		[SerializeField] private Vector2 _absoluteSize;
		[SerializeField] private AnimationCurve _scaleByDuration;
        
		private void FixedUpdate()
		{
			var duration = _system.time / _system.main.duration;
			var size = _absoluteSize * _scaleByDuration.Evaluate(duration);
			UpdateCollider(size.x, size.y);        
		}

		protected abstract void UpdateCollider(float x, float y);
	}
}