using Bones.Gameplay.Events.Args;
using UnityEngine;

namespace Bones.Gameplay.Entities
{
	public class Boss : Enemy
    {
		[SerializeField] private bool _miniBoss;

        public override bool HandleDistance(float distance)
        {
            base.HandleDistance(distance);
            return false;
        }

        protected override void OnSpawned()
        {
            base.OnSpawned();
            Publisher.Publish(new BossSpawnedArgs { Position = Position });
        }

        protected override void OnKilled()
        {
            base.OnKilled();
            Publisher.Publish(new BossDiedArgs() { MiniBoss = _miniBoss });
        }
    }
}