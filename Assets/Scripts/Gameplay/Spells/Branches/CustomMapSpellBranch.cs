using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Stats.Containers.Builders.Wrappers;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Spells.Classes
{
	public abstract class CustomMapSpellBranch : BaseSpellBranch
	{
		// TODO: add validation
		[SerializeField, Multiline(5)] private string _description;
		[SerializeReference] private SerializableWrappedStatsConfiguration _statsConfiguration;

		public override string Description => _description;

		protected sealed override IMutableTrace AppendTraceOnInject(DiContainer container, IMutableTrace trace)
		{
			if (_statsConfiguration == null)
				return base.AppendTraceOnInject(container, trace);
			
			container.Inject(_statsConfiguration);
			return trace.Add(_statsConfiguration.Create());
		}
	}
}