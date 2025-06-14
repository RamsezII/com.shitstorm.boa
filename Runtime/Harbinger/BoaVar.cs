namespace _BOA_
{
    public class BoaVar
    {
        public readonly string name;
        public object value;

        //----------------------------------------------------------------------------------------------------------

        public BoaVar(in string name, in object value)
        {
            this.name = name;
            this.value = value;
        }
    }
}