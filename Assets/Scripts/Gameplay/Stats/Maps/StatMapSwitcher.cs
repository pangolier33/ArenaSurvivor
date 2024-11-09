using System.Collections.Generic;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Stats.Containers;
using Bones.Gameplay.Stats.Containers.Graphs;

namespace Bones.Gameplay.Spells.Classes
{
	public class StatMapSwitcher : IStatMap
	{
		private IStatMap _statMapImplementation;

		public StatMapSwitcher(IStatMap statMapImplementation)
		{
			_statMapImplementation = statMapImplementation;
		}

		public IEnumerable<KeyValuePair<StatName, IStat>> GetAll() => _statMapImplementation.GetAll();
		public bool TryGet(StatName name, out IStat stat) => _statMapImplementation.TryGet(name, out stat);
		public IStat Get(StatName name) => _statMapImplementation.Get(name);
		public bool Contains(StatName name) => _statMapImplementation.Contains(name);
		public void Switch(IStatMap otherMap) => _statMapImplementation = otherMap;
		public override string ToString()
		{
			return $"StatMap Wrapper: {_statMapImplementation}";
		}
	}
}