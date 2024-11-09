using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Effects.Transitive.Depth;
using Bones.Gameplay.Events.Args;
using Bones.Gameplay.Factories.Pools.Mono;
using Bones.Gameplay.Items;
using Bones.Gameplay.Players;
using Bones.Gameplay.Stats.Containers;
using Bones.Gameplay.Stats.Containers.Graphs;
using Bones.Gameplay.Stats.Units;
using Bones.Gameplay.Stats.Utils;
using Bones.Gameplay.Waves.Spawning.Amounts;
using Railcar.Time;
using UniRx;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Entities
{
	public class Enemy : MonoPoolBridge<Enemy.Data, Enemy>, ISpellActor
    {
        [SerializeField] private float _blinkDelay = 0.1f;
        [SerializeField] private AnimationCurve _sizeByTime;
        private Rigidbody2D _rigidbody;
        private Collider2D _collider;
        private SpriteRenderer _renderer;
        private bool _isAttackingCooldown;
        private Vector3 _baseSize;
		private Vector2 _target;

        private Material _hitMaterial;
        private Material _defaultMaterial;
		private Material _deathMaterial;
		private IDisposable _hitSubscription;

        private SingletonEnemyAnimator _singletonEnemyAnimator;
        private float _enemyAnimatorBaseOffset;
        private ITimer _timer;
        private IDisposable _attackSubscription;
        private Value _lastHealth;
		private Value _maxHealth;

		private Player _player;
		public bool IsDead { get; private set; }
		public IStatMap Stats { get; private set; }
        public Pair Position => (Vector2)transform.position + _collider.offset / 2;
        protected IMessagePublisher Publisher { get; private set; }
        public AnimationCurve SizeCurve => _sizeByTime;
		public event Action Spawned;

		[Inject]
		private void Inject(Player player)
		{
			_player = player;
		}

		public virtual bool HandleDistance(float distance)
        {
	        return distance > Settings.DespawningDistance;
        }

		public virtual void OnCollisionStayImpl(Player player)
		{
			if (!IsSpawned || IsDead)
				return;
			if (_isAttackingCooldown)
				return;

			var playerDamageStat = (IGetStat<Value>)Stats.Get(StatName.Damage);
			var playerDamageAmount = playerDamageStat.Get();
			var playerHealthStat = (ISubtractStat<Value>)player.Stats.Get(StatName.OverallHealth);
			playerHealthStat.Subtract(playerDamageAmount);

			var selfDamageStat = (IGetStat<Value>)Stats.Get(StatName.FakeDamage);
			var selfDamageAmount = selfDamageStat.Get();
			var playerThornsStat = (IGetStat<Value>)player.Stats.Get(StatName.Thorns);
			var playerThornsValue = playerThornsStat.Get();
			var selfHealthStat = (ISubtractStat<Value>)Stats.Get(StatName.OverallHealth);
			selfHealthStat.Subtract(playerThornsValue * selfDamageAmount);

			_isAttackingCooldown = true;
			_attackSubscription?.Dispose();
			_attackSubscription = _timer.Mark(Stats.GetValue<Value>(StatName.AttackDelay), ResetAttackingCooldown);
		}

        private void ResetAttackingCooldown(float _)
        {
	        _isAttackingCooldown = false;
	        _attackSubscription = null;
        }

        public virtual void Move(Vector2 direction, Vector2 target)
        {
            if (float.IsNaN(direction.x) ||
                float.IsNaN(direction.y) ||
                float.IsInfinity(direction.x) ||
                float.IsInfinity(direction.y))
                return;
            
            var scale = transform.localScale;
            transform.localScale = new(scale.x, _singletonEnemyAnimator.Scale + _enemyAnimatorBaseOffset, scale.z);
            _renderer.flipX = direction.x > 0;
            
            var velocity = direction.normalized * Stats.GetValue<Value>(StatName.MovingSpeed);
            _rigidbody.AddForce(velocity, ForceMode2D.Force);

			_target = target;
        }

        protected override void OnSpawned()
        {
			IsDead = false;
            _isAttackingCooldown = false;
            _renderer.material = _defaultMaterial;
            Publisher.Publish(new EnemySpawnedArgs { Position = Position, Subject = this });

            Stats = Settings.StatsFactory.Create();
            var healthReadOnlyStat = (ReadOnlyStat<Value>)Stats.Get(StatName.OverallHealth);
            healthReadOnlyStat.Skip(1).Subscribe(OnHealthUpdated);
			_lastHealth = healthReadOnlyStat.BaseValue;
			_maxHealth = healthReadOnlyStat.BaseValue;

			var entitySizeOffset = Settings.SizeOffsetResolver;
            _enemyAnimatorBaseOffset = entitySizeOffset.GetAmount();
            transform.localScale = _baseSize + new Vector3(_enemyAnimatorBaseOffset, 0);

			Spawned?.Invoke();
			//_player = Settings.Trace.Get<Player>();
		}

        protected override void OnDespawned()
        {
			IsDead = true;
			_attackSubscription?.Dispose();
			_attackSubscription = null;

            _hitSubscription?.Dispose();
            _hitSubscription = null;
        }
        
        [Inject]
        private void Inject(
            [Inject] IMessagePublisher publisher,
            [Inject(Id = TimeID.Fixed)] ITimer timer,
            [Inject(Id = MaterialId.EnemyHit)] Material hitMaterial,
            [Inject(Id = MaterialId.EnemyDefault)] Material defaultMaterial,
			[Inject(Id = MaterialId.EnemyDeath)] Material deathMaterial,
			[Inject] IFactory<Enemy, SingletonEnemyAnimator> animatorFactory)
        {
            _timer = timer;
            Publisher = publisher;
            _hitMaterial = hitMaterial;
            _defaultMaterial = defaultMaterial;
			_deathMaterial = new Material(deathMaterial);
            
            _singletonEnemyAnimator = animatorFactory.Create(this);
        }
        
        protected virtual void Awake()
        {
            gameObject.name = $"{gameObject.name.Replace("(Clone)", "")} {Guid.NewGuid().ToString()[..8]}";
            _baseSize = transform.localScale;
            _rigidbody = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
            _renderer = GetComponentInChildren<SpriteRenderer>();
        }

		public void AddImpulse(Vector2 position, float impulse)
		{
			Vector2 d = (Vector2)transform.position - position;
			_rigidbody.AddForce(d.normalized * impulse, ForceMode2D.Impulse);
		}

		private void OnHealthUpdated(Value healthValue)
		{
			if (IsDead)
				return;

			if (_blinkDelay <= 0)
				throw new InvalidOperationException($"Blink delay for {gameObject.name} is not configured properly");
			Publisher.Publish(new EnemyDamagedArgs 
			{ 
				Position = Position,
				Value = (int) (_lastHealth.Base - healthValue.Base),
				RelativeValue = (int) ((_lastHealth.Base - healthValue.Base) / _maxHealth.Base)
			});

			_lastHealth = healthValue;
			_hitSubscription?.Dispose();
			_hitSubscription = null;

			if (healthValue <= Value.Zero)
			{
				IsDead = true;
				_renderer.material = _deathMaterial;
				const float fadeTime = 0.35f;
				const float deathImpulse = 8.0f;
				_deathMaterial.SetFloat("_DeathTime", Time.timeSinceLevelLoad);
				_deathMaterial.SetFloat("_FadeTime", fadeTime);
				AddImpulse(_target, deathImpulse);
				Invoke("OnKilled", fadeTime);
			}
			else
			{
				_renderer.material = _hitMaterial;
				_hitSubscription = _timer.Mark(_blinkDelay, _ =>
				{
					_renderer.material = _defaultMaterial;
					_hitSubscription = null;
				});
			}
		}

		protected virtual void OnKilled()
        {
            Settings.DropConfig?.Spawn(Position);
            
            Publisher.Publish(new EnemyDiedArgs
            {
                Subject = this,
                Position = Position
            });

			Dispose();

			AddEnemyKillScoresForStamina();
		}
        
        public struct Data
        {
            public DropConfig DropConfig { get; init; } 
            public IAmountFResolver SizeOffsetResolver { get; init; }
            public IFactory<IStatMap> StatsFactory { get; init; }
            public float DespawningDistance { get; init; }
        }

		private void AddEnemyKillScoresForStamina()
		{
			var staminaParametrs = ((IGetStat<StaminaValue>)_player.Stats.Get(StatName.StaminaParametrs)).Get();
			var staminaEnemyKillScores = new Value(staminaParametrs.enemyKillScores);
			var playerStamina = (UncertainStat<Value, Value, Value>)_player.Stats.Get(StatName.Stamina);
			var staminaReached = (Stat<bool>)_player.Stats.Get(StatName.StaminaZeroReached);
			staminaParametrs.Add(playerStamina,staminaEnemyKillScores,staminaReached);
		}
    }
}