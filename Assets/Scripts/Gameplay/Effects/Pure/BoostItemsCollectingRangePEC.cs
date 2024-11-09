using System;
using Bones.Gameplay.Events.Callbacks.Templates.Base;
using Bones.Gameplay.Items;
using Bones.Gameplay.Stats.Units;
using Zenject;

namespace Bones.Gameplay.Effects.Pure
{
	[Serializable]
	public sealed class BoostItemsCollectingRangePEC : BoostFromStatPEC<Value>, IInjectable
	{
		private ItemCollectingSettings _settings;
		
		protected override void Apply(Value amplifier) => _settings.CollectingDistance += amplifier;
		protected override void Dispel(Value amplifier) => _settings.CollectingDistance -= amplifier;
		
		[Inject]
		private void Inject(ItemCollectingSettings settings)
		{
			_settings = settings;
		}
	}
}