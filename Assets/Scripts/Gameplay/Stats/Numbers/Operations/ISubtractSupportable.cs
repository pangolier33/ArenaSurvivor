namespace Bones.Gameplay.Stats.Units.Operations
{
	public interface ISubtractSupportable<in TOther, out TResult>
	{
		TResult Subtract(TOther right);
	}
}