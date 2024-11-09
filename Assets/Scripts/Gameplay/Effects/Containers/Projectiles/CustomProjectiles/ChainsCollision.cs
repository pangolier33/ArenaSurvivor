using UnityEngine;

namespace Bones.Gameplay.Effects.Containers.Projectiles
{
	public class ChainsCollision : MonoBehaviour
    {
		Chains _chains;

        // Start is called before the first frame update
        void Awake()
        {
			_chains = GetComponentInParent<Chains>();        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

		private void OnTriggerEnter2D(Collider2D other)
		{
			_chains.OnChainsCollision(other);
		}

	}
}
