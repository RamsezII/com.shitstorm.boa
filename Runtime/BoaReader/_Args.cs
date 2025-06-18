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

                if (TryPeekChar_match(')', out _))
                    goto success;

                error = $"expected ',' or ')' after argument '{argument}'";
            }

            read_i = read_old;
            return false;

        success:
            LintToThisPosition(lint);
            return true;
        }

        public bool TryReadString_match(in string match, in bool as_function_argument, in Color lint, in bool ignore_case = true, in bool add_to_completions = true) => TryReadString_matches_out(out _, as_function_argument, lint: lint, ignore_case: ignore_case, add_to_completions: add_to_completions, matches: match);
        public bool TryReadString_match_out(out string value, in bool as_function_argument, in string match, in Color lint, in bool ignore_case = true, in bool add_to_completions = true) => TryReadString_matches_out(out value, as_function_argument, lint: lint, ignore_case: ignore_case, add_to_completions: add_to_completions, matches: match);
        public bool TryReadString_matches_out(out string value, in bool as_function_argument, in Color lint, in bool ignore_case = true, in bool add_to_completions = true, in string skippables = _empties_, in string stoppers = _stoppers_, params string[] matches)
        {
            StringComparison ordinal = ignore_case.ToOrdinal();
            int read_old = read_i;
            value = null;

            if (skippables == null || HasNext(true, skippables: skippables))
            {
                if (add_to_completions)
                    if (IsOnCursor())
                        completions.UnionWith(matches);

                value = string.Empty;
                while (TryPeekChar_out(out char peek, out _, skippables: null))
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

                                    if (as_function_argument)
                                        if (TryReadChar_match(',', lint: lint_theme.argument_coma))
                                            return true;

                                    if (!as_function_argument || !strict_syntax)
                                        return true;

                                    if (TryPeekChar_match(')', out _))
                                        return true;

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