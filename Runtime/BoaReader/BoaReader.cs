using System;
using System.Linq;

namespace _BOA_
{
    [Serializable]
    public sealed partial class BoaReader
    {
        public enum Sources : byte
        {
            Undefined,
            CommandLine,
            Script,
        }

        public readonly Sources mode;
        public readonly string text;
        public int start_i, read_i;
        public string last_arg;

#if UNITY_EDITOR
        readonly int _text_length;
#endif

        public bool IsScript => mode == Sources.Script;
        public bool IsCommandLine => mode == Sources.CommandLine;

        //----------------------------------------------------------------------------------------------------------

        public BoaReader(in Sources mode, in string text, in int read_i = 0)
        {
            this.mode = mode;
            this.read_i = read_i;
            this.text = text;
#if UNITY_EDITOR
            _text_length = text.Length;
#endif
        }

        //----------------------------------------------------------------------------------------------------------

        public char Peek() => text[read_i];
        public void Peek(out char c) => c = text[read_i];
        public bool HasNext(in string skippables = _empties_) => text.HasNext(ref read_i);

        public bool TryPeek(out char c, in string skippables = _empties_)
        {
            int read_old = read_i;

            if (skippables != null)
                HasNext(skippables);

            if (read_i < text.Length)
            {
                c = text[read_i];
                return true;
            }

            c = '\0';

            read_i = read_old;
            return false;
        }

        public bool TryPeekChar(in char expected_value, in string skippables = _empties_)
        {
            int read_old = read_i;

            if (TryPeek(out char c, skippables) && c == expected_value)
                return true;

            read_i = read_old;
            return false;
        }

        public bool TryReadChar(in char expected_value, in string skippables = _empties_, in bool ignore_case = true)
        {
            if (TryPeekChar(expected_value, skippables))
            {
                ++read_i;
                return true;
            }
            return false;
        }

        public bool TryReadChar(out char value, in string expected_values, in string skippables = _empties_, in bool ignore_case = false)
        {
            int read_old = read_i;

            if (TryPeek(out value, skippables) && expected_values.Contains(value, ignore_case ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
            {
                ++read_i;
                return true;
            }

            read_i = read_old;
            return false;
        }

        public bool TryReadArgument(out string argument, out string error, in bool check_parenthesis = true)
        {
            int read_old = read_i;

            error = null;
            if (TryReadArgument(text, out start_i, ref read_i, out argument))
            {
                last_arg = argument;
                if (!check_parenthesis || !IsScript)
                    return true;

                if (TryReadChar(',') || TryPeekChar(')'))
                    return true;

                error = $"expected ',' or ')' after argument '{argument}'";
            }

            read_i = read_old;
            return false;
        }

        public bool TryReadMatch(out string value, string match) => TryReadMatch(out value, true, _empties_, match);
        public bool TryReadMatch(out string value, in bool ignore_case, in string skippables = _empties_, params string[] matches)
        {
            int read_old = read_i;

            if (skippables != null)
                HasNext(skippables);

            if (TryReadArgument(text, out start_i, ref read_i, out value))
                if (matches.Contains(value, ignore_case ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal))
                {
                    last_arg = value;
                    return true;
                }

            read_i = read_old;
            return false;
        }
    }
}