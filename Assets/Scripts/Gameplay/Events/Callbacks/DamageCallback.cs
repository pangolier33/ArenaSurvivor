using Bones.Gameplay.Entities;
using Bones.Gameplay.Events.Args;
using Bones.Gameplay.Factories.Pools.Base;
using JetBrains.Annotations;

namespace Bones.Gameplay.Events.Callbacks
{
	public class DamageCallback : BaseCallback<EnemyDamagedArgs>
	{
		private readonly IPool<EnemyDamagedArgs, Damage> _pool;

		public DamageCallback([NotNull] IPool<EnemyDamagedArgs, Damage> pool)
		{
			_pool = pool;
		}

		public override void OnNext(EnemyDamagedArgs value)
		{
			var source = _pool.Create(value);
		}
	}
}