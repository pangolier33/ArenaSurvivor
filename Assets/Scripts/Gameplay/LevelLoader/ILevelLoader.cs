using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bones.Gameplay.LevelLoader
{
	public interface ILevelLoader
	{
		Task Load(int level);
		List<LevelData> levels { get; set; }
	}
}