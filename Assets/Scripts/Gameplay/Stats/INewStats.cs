using Bones.Gameplay.Stats.Containers.Graphs;

namespace Bones.Gameplay.Effects.Transitive.Depth
{
	public interface INewStats
	{
		float Get(StatName name);
		void Add(StatName name, float value);
		void Subtract(StatName name, float value);
		void TakeDamage(float amount);
		void Kill();
		void Heal(float amount);
	}
}