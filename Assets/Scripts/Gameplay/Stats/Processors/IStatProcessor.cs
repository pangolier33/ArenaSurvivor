namespace Bones.Gameplay.Stats.Processors
{
	public interface IStatProcessor<T>
	{
		T Process(T from);
	}
}