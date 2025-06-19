namespace _BOA_
{
    public sealed partial class BoaSignal
    {
        public readonly SIG_FLAGS_new flags;
        public readonly BoaReader reader;

        //--------------------------------------------------------------------------------------------------------------

        public BoaSignal(in SIG_FLAGS_new flags, in BoaReader reader)
        {
            this.flags = flags;
            this.reader = reader;
        }
    }
}