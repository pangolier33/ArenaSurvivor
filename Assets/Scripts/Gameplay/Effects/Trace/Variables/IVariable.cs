namespace Bones.Gameplay.Effects.Provider.Variables
{
    public interface IVariable<out T> : ITag
    {
        T Value { get; }
    }
}