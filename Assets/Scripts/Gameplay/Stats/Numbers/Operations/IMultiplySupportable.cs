namespace Bones.Gameplay.Stats.Units.Operations
{
	public interface IMultiplySupportable<in TOther, out TResult>
	{
		TResult Multiply(TOther right);
	}
}