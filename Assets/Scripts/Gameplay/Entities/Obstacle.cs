using Bones.Gameplay.Players;
using UnityEngine;

namespace Bones.Gameplay.Entities
{
    public class Obstacle : Enemy
    {
        public override void Move(Vector2 _, Vector2 t) { }

		public override void OnCollisionStayImpl(Player player) { }
	}
}