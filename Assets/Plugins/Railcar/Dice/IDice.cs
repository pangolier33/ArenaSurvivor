namespace Railcar.Dice
{
    public interface IDice
    {
        float Get();
    }

    public static class DiceExtensions
    {
        public static bool Roll(this IDice dice, float chance) => dice.Get() < chance;
    }
}