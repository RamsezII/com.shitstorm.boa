using System;

namespace _BOA_
{
    [Serializable]
    public sealed partial class BoaReader
    {
        public readonly string text;
        public int start_i, read_i;
        public string last_arg;

        public const string
            blacklist_boa = " \n\r{}():;'\"";

        //----------------------------------------------------------------------------------------------------------

        public BoaReader(in string text, in int read_i = 0)
        {
            this.read_i = read_i;
            this.text = text;
        }

        //----------------------------------------------------------------------------------------------------------

        public char Peek() => text[read_i];
        public void Peek(out char c) => c = text[read_i];
        public bool HasNext() => text.HasNext(ref read_i);

        public bool TryPeek(out char c)
        {
            if (read_i >= text.Length)
            {
                c = '\0';
                return false;
            }
            c = text[read_i];
            return true;
        }

        public bool TryReadChar(in char expected_value)
        {
            if (TryPeek(out char c) && c == expected_value)
            {
                ++read_i;
                return true;
            }
            return false;
        }

        public bool TryReadArgument(out string argument)
        {
            if (TryReadArgument(text, out start_i, ref read_i, out argument))
            {
                last_arg = argument;
                return true;
            }
            return false;
        }

        public bool TryReadArgument(in string match, in bool ignore_case)
        {
            if (TryReadArgument(text, out start_i, ref read_i, out string argument))
                if (match.Equals(argument, ignore_case ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
                {
                    last_arg = argument;
                    return true;
                }
                else
                    read_i = start_i;
            return false;
        }
    }
}