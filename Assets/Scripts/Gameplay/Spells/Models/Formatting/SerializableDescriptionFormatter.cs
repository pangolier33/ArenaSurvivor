using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Bones.Gameplay.Spells.Classes
{
	[Serializable]
	public class SerializableDescriptionFormatter : IDescriptionFormatter
	{
		[SerializeField] private string _descriptionFormat;
		[SerializeField] private string _statSeparator;
		[SerializeField] private string _statPlainLineFormat;
		[SerializeField] private string _statTransitionLineFormat;
        
		[SerializeField]
		[HideLabel]
		[InlineProperty]
		private StatLineFormatter _lineFormatter;

		public string FormatPlain(string description, IEnumerable<IStatInfo> stats) => string.Format(
			_descriptionFormat,
			description,
			string.Join(
				_statSeparator,
				stats.Select(stat => string.Format(
					_statPlainLineFormat,
					_lineFormatter.GetIcon(stat.Name),
					_lineFormatter.Format(stat.Name, stat.Builder)
				))
			)
		);

		public string FormatTransitional(string description, IEnumerable<IStatInfo> fromStats, IEnumerable<IStatInfo> toStats) => string.Format(
			_descriptionFormat,
			description,
			string.Join(
				_statSeparator,
				fromStats.Concat(toStats)
					.GroupBy(x => x.Name)
					.Select(group => string.Format(
							_statTransitionLineFormat,
							_lineFormatter.GetIcon(group.Key),
							_lineFormatter.Format(group.Key, group.ElementAt(0).Builder),
							_lineFormatter.Format(group.Key, group.ElementAt(1).Builder)
						)
					)
			)
		);
	}
}