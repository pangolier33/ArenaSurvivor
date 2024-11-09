namespace Bones.Gameplay.Effects.Provider
{
	public interface IMutableTrace : ITrace
    {
        IMutableTrace Add<T>(T arg);
    }
}