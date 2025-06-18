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

            if (read_i < text.Length)
            {
                if (TryReadArgument(text, out int start_i, ref read_i, out argument, skippables: skippables, stoppers: stoppers))
                {
                    if (start_i <= cursor_i)
                    {
                        cpl_start = start_i;
                        cpl_end = read_i;
                    }

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
                cpl_start = read_old + 1;
                cpl_end = read_i;
            }

            read_i = read_old;
            argument = null;
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

            if (TryReadArgument(out value, as_function_argument: as_function_argument, lint: lint, skippables: skippables, stoppers: stoppers))
            {
                if (add_to_completions)
                    if (IsOnCursor())
                        completions.UnionWith(matches);

                for (int match_i = 0; match_i <= matches.Length; ++match_i)
                    if (match_i == matches.Length)
                        goto out_of_loop;
                    else
                    {
                        string match = matches[match_i];
                        if (match.Equals(value, ordinal))
                        {
                            last_arg = value = match;
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
                    }
            }
            else if (add_to_completions)
                if (IsOnCursor())
                    completions.UnionWith(matches);

                out_of_loop:
            read_i = read_old;
            return false;
        }
    }
}