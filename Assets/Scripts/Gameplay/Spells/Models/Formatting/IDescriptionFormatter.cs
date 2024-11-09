using System.Collections.Generic;

namespace Bones.Gameplay.Spells.Classes
{
	public interface IDescriptionFormatter
	{
		string FormatPlain(string description, IEnumerable<IStatInfo> stats);
		string FormatTransitional(string description, IEnumerable<IStatInfo> fromStats, IEnumerable<IStatInfo> toStats);
	}
}