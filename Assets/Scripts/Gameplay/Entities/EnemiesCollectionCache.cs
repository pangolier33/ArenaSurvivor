using System.Collections.Generic;
using System.Linq;
using Bones.Gameplay.Effects.Containers;
using Bones.Gameplay.Players;
using Railcar.Time;
using Zenject;

namespace Bones.Gameplay.Entities
{
	public class EnemiesCollectionCache
	{
		private const float DistanceLimit = 6;
		
		private readonly Dictionary<ISpellActor, CacheUnit> _map = new();
		private readonly float _maxLifetime;
		private readonly IClock _clock;
		private readonly IEnumerable<Enemy> _enemies;

		public EnemiesCollectionCache(float maxLifetime, [Inject(Id = TimeID.Fixed)] IClock clock, [Inject] IEnumerable<Enemy> enemies)
		{
			_maxLifetime = maxLifetime;
			_clock = clock;
			_enemies = enemies;
		}

		public IEnumerable<Enemy> GetSorted(ISpellActor actor)
		{
			return GetSorted(actor, DistanceLimit);
		}

		public IEnumerable<Enemy> GetSorted(ISpellActor actor, float distance)
		{
			if (!_map.ContainsKey(actor))
			{
				var collection = new LazyCollection<Enemy>(Query(_enemies, actor, distance).GetEnumerator());
				_map.Add(actor, new()
				{
					Timestamp = _clock.Current,
					Collection = collection,
				});

				return collection;
			}

			var unit = _map[actor];
			if (_clock.Current - unit.Timestamp < _maxLifetime)
				return unit.Collection;

			unit.Collection.Reset(Query(_enemies, actor, distance).GetEnumerator());
			_map[actor] = new()
			{
				Timestamp = _clock.Current,
				Collection = unit.Collection,
			};
			return unit.Collection;
		}

		private static IEnumerable<Enemy> Query(IEnumerable<Enemy> source, ISpellActor actor, float distance)
		{
			var center = actor.Position;
			return source.Where(target => !ReferenceEquals(target, actor))
			             .Where(enemy => enemy is not CircleOfExperience and not Obstacle)
			             .Where(enemy => (center - enemy.Position).Magnitude < distance)
			             .OrderBy(enemy => (float)(center - enemy.Position).Magnitude);
		}

		private readonly struct CacheUnit
		{
			public float Timestamp { get; init; }
			public LazyCollection<Enemy> Collection { get; init; }
		}
	}
}