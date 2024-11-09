using Bones.Gameplay.Stats.Containers;

namespace Bones.Gameplay
{
	/// <summary>
	/// Required for serializing references of different builder variations
	/// </summary>
	public interface IStatBuilder
	{
		IStat Build();
		IStat BuildUncertain();
		IStat BuildWrapper(IStat wrappedStat);
	}
}