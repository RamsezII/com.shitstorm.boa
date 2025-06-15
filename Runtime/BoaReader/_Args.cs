using System;

namespace _BOA_
{
    partial class BoaReader
    {
        public bool TryReadArgument(out string argument, in bool as_function_argument, in string skippables = _empties_, in string stoppers = _stoppers_)
        {
            int read_old = read_i;

            error = null;

            if (TryReadArgument(text, out start_i, ref read_i, out argument, skippables: skippables, stoppers: stoppers))
            {
                last_arg = argument;

                if (as_function_argument)
                    if (TryReadChar_match(','))
                        return true;

                if (!as_function_argument || !strict_syntax)
                    return true;

                if (TryPeekChar_match(')'))
                    return true;

                error = $"expected ',' or ')' after argument '{argument}'";
            }

            read_i = read_old;
            return false;
        }

        public bool TryReadString_match(in string match) => TryReadString_matches_out(out _, matches: match);
        public bool TryReadString_match_out(out string value, in string match) => TryReadString_matches_out(out value, matches: match);
        public bool TryReadString_matches_out(out string value, in string skippables = _empties_, in string stoppers = _stoppers_, params string[] matches)
        {
            StringComparison ordinal = StringComparison.OrdinalIgnoreCase;
            int read_old = read_i;
            value = null;

            if (skippables == null || HasNext(true, skippables: skippables))
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
                                    last_arg = value;
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