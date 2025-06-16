using System;
using UnityEngine;

namespace _BOA_
{
    partial class BoaReader
    {
        public bool TryReadArgument(out string argument, in bool as_function_argument, in Color lint, in string skippables = _empties_, in string stoppers = _stoppers_)
        {
            int read_old = read_i;

            error = null;

            if (TryReadArgument(text, out start_i, ref read_i, out argument, skippables: skippables, stoppers: stoppers))
            {
                last_arg = argument;

                if (as_function_argument)
                    if (TryReadChar_match(',', lint: lint_theme.argument_coma))
                        goto success;

                if (!as_function_argument || !strict_syntax)
                    goto success;

                if (TryPeekChar_match(')'))
                    goto success;

                error = $"expected ',' or ')' after argument '{argument}'";
            }

            read_i = read_old;
            return false;

        success:
            LintToThisPosition(lint);
            return true;
        }

        public bool TryReadString_match(in string match, in Color lint) => TryReadString_matches_out(out _, lint: lint, matches: match);
        public bool TryReadString_match_out(out string value, in string match, in Color lint) => TryReadString_matches_out(out value, lint: lint, matches: match);
        public bool TryReadString_matches_out(out string value, in Color lint, in string skippables = _empties_, in string stoppers = _stoppers_, params string[] matches)
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
                                {
                                    last_arg = value;
                                    LintToThisPosition(lint);
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