namespace _BOA_
{
    public sealed class Literal<T>
    {
        readonly object _value;
        public T Value => _value switch
        {
            Variable<T> v => v.value,
            _ => (T)_value,
        };

        //----------------------------------------------------------------------------------------------------------

        public Literal(in T value)
        {
            _value = value;
        }
    }

    public sealed class Variable<T>
    {
        public readonly string name;
        public T value;

        //----------------------------------------------------------------------------------------------------------

        public Variable(in string name, in T value = default)
        {
            this.name = name;
            this.value = value;
        }
    }
}