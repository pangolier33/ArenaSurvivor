using Bones.Gameplay.Effects.Provider;
using Bones.Gameplay.Players;
using Bones.Gameplay.Spells.Classes;
using Bones.Gameplay.Stats.Containers;
using Bones.Gameplay.Stats.Containers.Graphs;
using Bones.Gameplay.Stats.Numbers;
using Bones.Gameplay.Stats.Units;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Bones.Gameplay.Meta.Equipment
{
	[CreateAssetMenu(menuName = "Meta/EquipmentData")]
	public class EquipmentData : ScriptableObject, IEquipmentData
	{
		[SerializeField] private string _id;
		[SerializeField] private EquipmentType _type;
		[SerializeField] private string _nameId;
		[SerializeField] private string _descriptionId;
		[SerializeField] private Sprite _icon;
		[SerializeField] private ISpellModel _spellModel;

		public string Id => _id;
		public EquipmentType Type => _type;
		public string NameId => _nameId;
		public string DescriptionId => _descriptionId;
		public Sprite Icon => _icon;
		public ISpellModel SpellModel => _spellModel;

		[TableList(AlwaysExpanded = true, ShowPaging = false, DefaultMinColumnWidth = 50)]
		[SerializeField] private List<StatRow> _stats;

		public void ApplyStats(float multiplier, Player player)
		{
			foreach (var stat in _stats)
			{
				float statMultiplier = stat.Multiply ? multiplier : 1;
				var playerStat = player.Stats.Get(stat.Stat);
				IncreaseStat(playerStat, stat.Value * statMultiplier);
			}
		}

		private void IncreaseStat(IStat playerStat, float value)
		{
			if (playerStat is Stat<Value> valueStat)
				valueStat.Set(valueStat.BaseValue.Add(new Value(value)));
			else if (playerStat is Stat<TimedValue> timedStat)
				timedStat.Set(timedStat.BaseValue.Add(new Value(value)));
			else if (playerStat is Stat<Points> pointsStat)
				pointsStat.Set(pointsStat.BaseValue.Add(new Amount((int)value)));
			else if (playerStat is Stat<SpeedValue> speedStat)
				speedStat.Set(speedStat.BaseValue.Add(new Value(value)));
		}
	}
}
