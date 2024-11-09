using System.Collections.Generic;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Stats.Containers;
using Bones.Gameplay.Stats.Containers.Graphs;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
	public class UnitStatMap : IStatMap
	{
		private readonly StatName _name;
		private readonly IStat _stat;

		public UnitStatMap(StatName name, IStat stat)
		{
			_name = name;
			_stat = stat;
		}

		public IEnumerable<KeyValuePair<StatName, IStat>> GetAll()
		{
			yield return new KeyValuePair<StatName, IStat>(_name, _stat);
		}

		public bool TryGet(StatName name, out IStat stat)
		{
			if (Contains(name))
			{
				stat = Get(name);
				return true;
			}

			stat = null;
			return false;
		}

		public IStat Get(StatName name)
		{
			if (Contains(name))
				return _stat;
			throw new KeyNotFoundException($"Trying to get {name} stat from unit stat that holds {_name}");
		}

		public bool Contains(StatName name) => _name == name;

		public override string ToString()
		{
			return $"{_name}: {_stat}";
		}
	}
}