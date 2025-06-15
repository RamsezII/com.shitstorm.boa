using System;

namespace _BOA_
{
    [Serializable]
    public sealed partial class BoaReader
    {
        public bool strict_syntax;
        public readonly string text;
        public int start_i, read_i;
        public string last_arg;

#if UNITY_EDITOR
        readonly int _text_length;
        string toLog => text[..read_i] + "°" + text[read_i..];
#endif

        public string error, long_error;

        //----------------------------------------------------------------------------------------------------------

        public BoaReader(in bool strict_syntax, in string text, in int read_i = 0)
        {
            this.strict_syntax = strict_syntax;
            this.read_i = read_i;
            this.text = text;
#if UNITY_EDITOR
            _text_length = text.Length;
#endif
        }

        //----------------------------------------------------------------------------------------------------------

        public bool HasNext(in bool ignore_case = true, in string skippables = _empties_) => text.HasNext(ref read_i, ignore_case: ignore_case, skippables: skippables);
    }
}