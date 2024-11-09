using UnityEngine;

namespace Bones.Gameplay.Effects.Containers.Projectiles
{
    public class CapsuleColliderParticleSynchronizer : ParticleSynchronizer
    {
        [SerializeField] private CapsuleCollider2D _collider;
        
        protected override void UpdateCollider(float x, float y)
        {
            _collider.size = new Vector2(x, y);
        }
    }
}