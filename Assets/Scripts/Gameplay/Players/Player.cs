using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Effects.Transitive.Depth;
using Bones.Gameplay.Entities;
using Bones.Gameplay.Stats.Containers;
using Bones.Gameplay.Stats.Containers.Graphs;
using Bones.Gameplay.Stats.Units;
using Bones.Gameplay.Stats.Utils;
using Bones.Gameplay.Events.Args;
using Railcar.Time;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;
using UnityEngine.UI;

namespace Bones.Gameplay.Players
{
	public class Player : MonoBehaviour, ISpellActor
    {
        private static readonly int Move1 = Animator.StringToHash("move");
        private static readonly int MoveX = Animator.StringToHash("moveX");
        private static readonly int s_animatorIsBoosted = Animator.StringToHash("IsBoosted");
        
        [SerializeField] private AudioClip[] _clips;
        [SerializeField] private float _audioDelay;
        [SerializeField] private AudioSource _source;
        [SerializeField] private float _blinkDelay;
        [SerializeField] private AnimationCurve _sizeByTime;
        private float _audioCooldown;

		private Animator _animator;
        private Collider2D _collider;
        
        private SpriteRenderer _renderer;
        private Material _hitMaterial;
        private Material _defaultMaterial;
        private ITimer _timer;
        private IDisposable _hitSubscription;
        private SingletonEnemyAnimator _singletonPlayerAnimator;

        private IStatMap _playerMap;
        
        public IStatMap Stats { get; private set; }
        public Pair Position => _collider.bounds.center;
        public AnimationCurve SizeCurve => _sizeByTime;
        public IMessagePublisher Publisher { get; private set; }
        public Transform graphicsTransform => _animator.transform;

		//+++hero speed test
		//private Text _kJoystic;
		//private Text _kStatSpeed;
		//private Text _totalSpeed;
		//private Text _stamina;
		//---hero speed test

		[Inject]
        private void Inject(
			[Inject] IMessagePublisher publisher,
            [Inject] IStatMap stats, 
            [Inject(Id = TimeID.Fixed)] ITimer timer,
            [Inject(Id = MaterialId.PlayerHit)] Material hitMaterial,
            [Inject(Id = MaterialId.PlayerDefault)] Material defaultMaterial,
            [Inject] IFactory<Player, SingletonEnemyAnimator> animatorFactory)
        {
            _hitMaterial = hitMaterial;
            _defaultMaterial = defaultMaterial;
            Stats = stats;
            _timer = timer;
			Publisher = publisher;
			_singletonPlayerAnimator = animatorFactory.Create(this);
		}

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
            _collider = GetComponentInChildren<Collider2D>();
            _renderer = GetComponentInChildren<SpriteRenderer>();
            Stats.Get(StatName.OverallHealth).ObserveOnSubtract<Value>().Subscribe(OnDamaged);
            _renderer.material = _defaultMaterial;

			_singletonPlayerAnimator.Init(_renderer.transform.localScale.y);

			//+++hero speed test
			//_kJoystic = GameObject.Find("K joy").GetComponent<Text>();
			//_kStatSpeed = GameObject.Find("StatSpeed").GetComponent<Text>();
			//_stamina = GameObject.Find("Stamina").GetComponent<Text>();
			//_totalSpeed = GameObject.Find("Total").GetComponent<Text>();
			//---hero speed test
		}

		public void Move(Vector2 direction, float JoySpeedModifier, float timeDelta)
        {
			var scale = _animator.transform.localScale;
			_animator.transform.localScale = new(scale.x, _singletonPlayerAnimator.Scale, scale.z);

			if (JoySpeedModifier == 0 ||
				(Mathf.Approximately(direction.x, 0) && Mathf.Approximately(direction.y, 0)))
            {
                _animator.SetBool(Move1, false);
                _animator.SetBool(s_animatorIsBoosted, false);
                return;
            }

            _audioCooldown += timeDelta;
            if (_audioCooldown > _audioDelay)
            {
                _audioCooldown = 0;
                _source.PlayOneShot(_clips[Random.Range(0, _clips.Length)]);
            }

			var speed = ((IGetStat<SpeedValue>)Stats.Get(StatName.MovingSpeed)).Get();
            _animator.SetBool(Move1, true);
            _animator.SetFloat(MoveX, direction.x > 0 ? 1 : -1);
            _animator.SetBool(s_animatorIsBoosted, speed.IsBoosted);

			var resultSpeed = speed.Amplifier * JoySpeedModifier * GetStaminaSlowDownParameter();

			_animator.SetFloat("SpeedMultip", Mathf.Max(0.5f,JoySpeedModifier));
			//print("move: " + JoySpeedModifier + " SpeedMultip:" + Mathf.Max(0.5f, JoySpeedModifier));

			transform.Translate(resultSpeed * timeDelta * direction);

			//+++hero speed test
			//_kJoystic.text = "K joy: " + JoySpeedModifier;
			//_kStatSpeed.text = "Stat speed: " + speed.Amplifier;
			//_stamina.text = "Stamina: " + ((IGetStat<Value>)Stats.Get(StatName.Stamina)).Get().Base;
			//_totalSpeed.text = "Total: " + resultSpeed;
			//---hero speed test
		}

		private float GetStaminaSlowDownParameter()
		{		
			if (((IGetStat<bool>)Stats.Get(StatName.StaminaZeroReached)).Get())
			{
				return ((IGetStat<StaminaValue>)Stats.Get(StatName.StaminaParametrs)).Get().slowDownParameter;
			} else
				return 1;
		}

		private void OnDamaged(Value _)
        {
            _hitSubscription?.Dispose();
            _renderer.material = _hitMaterial;
            _hitSubscription = _timer.Mark(_blinkDelay, AnimateDamaging);
            
            var isDead = Stats.GetValue<Value>(StatName.OverallHealth) <= Value.Zero;
            if (isDead)
	            OnDying();
        }

        private void AnimateDamaging(float _)
        {
	        _renderer.material = _defaultMaterial;
	        _hitSubscription = null;
        }
        
        private void OnDestroy()
        {
            _hitSubscription?.Dispose();
        }

        private void OnDying()
        {
	        Publisher.Publish(new PlayerDiedArgs() { Player = this });
        }

		protected void OnCollisionStay2D(Collision2D other)
        {
            if (!other.gameObject.TryGetComponent(out Enemy enemy))
                return;

			enemy.OnCollisionStayImpl(this);
        }
	}
}
