using System;
using System.Collections.Generic;
using Bones.Gameplay.Factories.Pools.Mono;
using Bones.Gameplay.Flocks.Pulling;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Items
{
	public sealed class Item : MonoPoolBridge<Item>, IPullingEntity<Item>
	{
        [SerializeField] private float _appearingDuration;
        
        private ItemCollectingSettings _settings;
        private IDisposable _animationSubscription;
        private float _animationTime;
		private float _pullingTime;
            
        public event EventHandler Disposed; 

        public ICollection<Item> Cluster { get; set; }
        public Vector2 Position => transform.position;
		public bool Moving { get; private set; }

		public bool IsDespawningDistance(float distance)
		{
			return distance >= _settings.DespawningDistance;
		}

		public bool IsCollectingDistance(float distance)
		{
			return distance <= _settings.CollectingDistance;
		}

		public void Move(Vector2 force, float deltaTime)
        {
			Moving = true;
			transform.Translate(force * _settings.PullingForce * _settings.PullingCurve.Evaluate(_pullingTime));
			_pullingTime += deltaTime;
        }

		public void Collect()
		{
			Disposed?.Invoke(this, EventArgs.Empty);
			Dispose();
		}

		public override void Dispose()
		{
			_pullingTime = 0f;
			Moving = false;
			base.Dispose();
		}

		protected override void OnSpawned()
        {
        }

        protected override void OnDespawned()
        {
        }

        [Inject]
        private void Inject(ItemCollectingSettings settings)
        {
            _settings = settings;
        }
	}
}