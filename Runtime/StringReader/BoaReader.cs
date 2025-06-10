using System;
using System.Linq;

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

        public bool TryPeek(out char c, in bool skip_empties = true)
        {
            if (skip_empties)
                HasNext();

            if (read_i < text.Length)
            {
                c = text[read_i];
                return true;
            }

            c = '\0';
            return false;
        }

        public bool TryReadChar(in char expected_value, in bool skip_empties = true)
        {
            if (skip_empties)
                HasNext();

            if (TryPeek(out char c) && c == expected_value)
            {
                ++read_i;
                return true;
            }

            return false;
        }

        public bool TryReadChar(out char value, in string expected_values, in bool ignore_case = false, in bool skip_empties = true)
        {
            if (skip_empties)
                HasNext();

            if (TryPeek(out value) && expected_values.Contains(value, ignore_case ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
            {
                ++read_i;
                return true;
            }

            value = default;
            return false;
        }

        public bool SkipUntil(in char expected_value)
        {
            while (read_i < text.Length)
            {
                if (TryPeek(out char c, false) && c == expected_value)
                {
                    ++read_i;
                    return true;
                }
                ++read_i;
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

        public bool TryReadMatch(in string match, in bool ignore_case)
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

        public bool TryReadMatch(out string match, in bool ignore_case, in bool skip_empties = true, params string[] matches)
        {
            if (skip_empties)
                HasNext();

            if (TryReadArgument(text, out start_i, ref read_i, out match))
                if (matches.Contains(match, ignore_case ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal))
                {
                    last_arg = match;
                    return true;
                }
                else
                    read_i = start_i;

            return false;
        }
    }
}