using Bones.Gameplay.Meta;
using System.Threading.Tasks;

namespace Bones.Gameplay.Saving
{
	public interface ILoader
	{
		Task<ISave> Load();
		bool Loaded { get; set; }
	}
}
