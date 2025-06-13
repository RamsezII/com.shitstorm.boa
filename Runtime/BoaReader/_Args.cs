using System;

namespace _BOA_
{
    partial class BoaReader
    {
        public bool TryReadArgument(out string argument, out string error, in string skippables = _empties_, in string stoppers = _stoppers_, in bool as_function_argument = true)
        {
            int read_old = read_i;

            error = null;

            if (TryReadArgument(text, out start_i, ref read_i, out argument, skippables: skippables, stoppers: stoppers))
            {
                last_arg = argument;
                if (!as_function_argument || !strict_syntax)
                    return true;

                if (TryReadChar_match(',') || TryPeekChar_match(')'))
                    return true;

                error = $"expected ',' or ')' after argument '{argument}'";
            }

            read_i = read_old;
            return false;
        }

        public bool TryReadMatch(out string value, string match) => TryReadMatch(out value, true, _empties_, match);
        public bool TryReadMatch(out string value, in bool ignore_case, in string skippables = _empties_, in string stoppers = _stoppers_, params string[] matches)
        {
            StringComparison ordinal = ignore_case.ToOrdinal();
            int read_old = read_i;
            value = null;

            if (skippables == null || HasNext(ignore_case: ignore_case, skippables: skippables))
            {
                value = string.Empty;
                while (TryPeekChar_out(out char peek, skippables: null))
                {
                    value += peek;
                    for (int i = 0; i <= matches.Length; ++i)
                        if (i == matches.Length)
                            goto out_of_loop;
                        else
                        {
                            string match = matches[i];
                            if (match.StartsWith(value, ordinal))
                            {
                                ++read_i;
                                if (match.Equals(value, ordinal))
                                {
                                    last_arg = value;
                                    return true;
                                }
                                break;
                            }
                        }
                }
            }
        out_of_loop:

            read_i = read_old;
            return false;
        }
    }
}