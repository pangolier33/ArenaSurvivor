using System;
using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Stats.Containers.Builders.Wrappers;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Spells.Classes
{
	[CreateAssetMenu]
	public sealed class LeveledSpellBranch : BaseSpellBranch
	{
		// todo: add validation
		[SerializeField, TabGroup("Levels", Paddingless = true)]
		[LabelText("Levels"), ListDrawerSettings(Expanded = true, DraggableItems = false, ShowPaging = false, ListElementLabelName = "@$property.Index+ \". Level\"")]
		private Settings[] _spellSettings;

		private readonly ReactiveProperty<int> _level = new();
		private StatMapSwitcher _mapSwitcher;
		private IDisposable _subscription;

		public override string Description => IsAvailable ? _spellSettings[_level.Value].Description : _spellSettings[_level.Value - 1].Description;
		public override bool IsActive => _level.Value > 0;
		public override bool IsAvailable => _level.Value < MaxLevel;
		public override float Weight => _spellSettings[_level.Value].Weight;

		public IReadOnlyReactiveProperty<int> Level => _level;
		public int MaxLevel => _spellSettings.Length;

		protected override IDisposable ApplyWhenAvailable()
		{
			_mapSwitcher.Switch(_spellSettings[_level.Value].MapBuilder.Create());
			if (!IsActive)
				_subscription = base.ApplyWhenAvailable();
			_level.Value++;
			return _subscription;
		}

		protected override void OnReset()
		{
			_level.Value = 0;
		}

		protected override IMutableTrace AppendTraceOnInject(DiContainer container, IMutableTrace trace)
		{
			foreach (var setting in _spellSettings)
				container.Inject(setting);
			return trace.Add(_mapSwitcher = new StatMapSwitcher(PlaceholderStatMap.Instance));
		}

		[Serializable]
		private class Settings
		{
			//[field: TabGroup("Meta", Paddingless = true)]
			[field: FoldoutGroup("Description")]
			[field: SerializeField]
			[field: Multiline(5)]
			public string Description { get; set; }

			[field: SerializeField]
			[field: InlineProperty]
			[field: HideLabel]
			//[field: TabGroup("Values", Paddingless = true)]
			public SerializableWrappedStatsConfiguration MapBuilder { get; private set; }

			//[field: TabGroup("Values", Paddingless = true)]
			[field: SerializeField] public float Weight { get; private set; } = 1.0f;

			[Inject]
			private void Inject(DiContainer container)
			{
				container.Inject(MapBuilder);
			}
		}
	}
}