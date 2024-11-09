using System.Collections.Generic;
using System.Linq;
using Bones.Gameplay.Players;
using Bones.Gameplay.Stats.Containers;
using Bones.Gameplay.Stats.Containers.Graphs;
using Bones.Gameplay.Stats.Numbers;
using Bones.Gameplay.Stats.Units;
using Bones.UI.Bindings.Base;
using Bones.UI.Presenters.New;
using UnityEngine;
using Zenject;

namespace UI.Presenters
{
	public class StatsListController : BaseBinder
	{
		[SerializeField] private List<StatName> _nameFilters;
		[SerializeReference] private IBinding _textBinding; 
		private Player _player;
		
		private void OnEnable()
		{
			OnNext(string.Join('\n', TransformPairsToFancyText(_player.Stats.GetAll())), _textBinding);
		}

		private IEnumerable<string> TransformPairsToFancyText(IEnumerable<KeyValuePair<StatName, IStat>> pairs)
		{
			foreach (var (name, stat) in pairs.Where(pair => _nameFilters.Contains(pair.Key)))
			{
				switch (stat)
				{
					case CooldownStat:
					{
						var points = ((IGetStat<Points>)stat).Get();
						var duration = (double)((IGetStat<Value>)stat).Get();
						yield return $"{name}: {points.Value} ({duration:F1}s)";
						break;
					}
					case IGetStat<Value> valueStat:
					{
						yield return $"{name}: {(double)valueStat.Get():F2}";
						break;
					}
					case IGetStat<Points> pointsStat:
					{
						var points = pointsStat.Get();
						yield return $"{name}: {points.Percent:P1}";
						break;
					}
					case IGetStat<Amount> amountStat:
					{
						yield return $"{name}: {(int)amountStat.Get()}";
						break;
					}
					case IGetStat<TimedValue> timedValueStat:
					{
						yield return $"{name}: {timedValueStat.Get().Amount * 2}/s";
						break;
					}
					case IGetStat<SpeedValue> speedValueStat:
					{
							yield return $"{name}: {speedValueStat.Get().Amplifier * 100}";
							break;
					}
				}
			}
		}
		
		[Inject]
		private void Inject(Player player)
		{
			_player = player;
		}
	}
}