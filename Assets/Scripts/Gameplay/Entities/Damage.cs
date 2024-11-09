using Bones.Gameplay.Events.Args;
using UnityEngine;

namespace Bones.Gameplay.Entities
{
	public abstract class Damage : MonoBehaviour
	{
		public abstract void SetValue(EnemyDamagedArgs enemyDamagedArgs);
		public abstract void Start();
		public abstract float LifeTime { get; }
	}
}
