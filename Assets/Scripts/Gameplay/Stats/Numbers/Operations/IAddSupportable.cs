namespace Bones.Gameplay.Stats.Units.Operations
{
	public interface IAddSupportable<in TOther, out TResult>
	{
		TResult Add(TOther right);
	}
}