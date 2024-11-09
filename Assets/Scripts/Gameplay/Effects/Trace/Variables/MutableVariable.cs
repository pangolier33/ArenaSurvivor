namespace Bones.Gameplay.Effects.Provider.Variables
{
    public class MutableVariable<T> : IVariable<T>
    {
        public MutableVariable(string id, T value)
        {
            ID = id;
            Value = value;
        }
        
        public string ID { get; }
        public T Value { get; set; }

        public override string ToString()
        {
            return $"{typeof(T).Name} {ID}: {Value}";
        }
    }
}