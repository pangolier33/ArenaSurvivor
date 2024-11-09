using System;
using System.Threading;
using Bones.Gameplay.Entities;
using Bones.Gameplay.Factories.Pools.Base;
using Bones.Gameplay.Utils;
using Bones.Gameplay.Waves.Spawning.Amounts;
using Bones.Gameplay.Waves.Spawning.Positions;
using Railcar.Time;
using Zenject;

namespace Bones.Gameplay.Waves
{
	public class DraftSpawner : IDisposable
    {
        private readonly Settings _settings;
        private IDisposable _subscription;
		private CancellationTokenSource _token;

		private DraftSpawner(
            [Inject(Id = TimeID.Fixed)] ITimer timer,
            Settings settings
        )
        {
            _settings = settings;
			if (settings.IsRepeating)
				_token = timer.MarkAndRepeat(settings.Delay, OnTriggered);
			else
				_subscription = timer.Mark(settings.Delay, OnTriggered);
        }

        private void OnTriggered(float time)
        {
            var requestedAmount = _settings.AmountResolver.GetAmount();
            
            if (_settings.IsRepeating)
                requestedAmount = Math.Min(_settings.Limit - _settings.Pool.Entities.Count, requestedAmount);


            for (var i = 0; i < requestedAmount; i++)
            {
                var position = _settings.PositionResolver.PickUp();
                var entity = _settings.Pool.Create(_settings.EntitySettings);
                entity.transform.position = position;
            }

			if (!_settings.IsRepeating)
				_subscription = null;
        }

        public void Dispose()
        {
			_subscription?.Dispose();
			_subscription = null;

			_token?.Cancel();
			_token?.Dispose();
			_token = null;
		}

        public struct Settings
        {
            public IPool<Enemy.Data, Enemy> Pool { get; init; }
            public Enemy.Data EntitySettings { get; init; }
            public IAmountResolver AmountResolver { get; init; }
            public IPositionResolver PositionResolver { get; init; }
            public bool IsRepeating { get; init; }
            public float Delay { get; init; }
            public int Limit { get; init; }
        }
    }
}