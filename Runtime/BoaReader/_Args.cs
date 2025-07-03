using System;
using System.Collections.Generic;
using UnityEngine;

namespace _BOA_
{
    partial class BoaReader
    {
        public const string
            _stoppers_ = " \n\r[]{}().,;'\"\\=+-*/%<>|&",
            _stoppers_types_ = " \n\r[]{}(),;'\"\\=-*/%<>|&",
            _stoppers_operators_ = " \n\r[]{}().,;'\"\\<>|&",
            _stoppers_options_ = " \n\r[]{}().,;'\"\\=+*/%<>|&",
            _stoppers_factors_ = " \n\r[]{}(),;'\"\\=+-*/%<>|&",
            _stoppers_paths = " \n\r[]{}(),;'\"=+-*%<>|&",
            _empties_ = " \t\n\r";

        //----------------------------------------------------------------------------------------------------------

        public static bool TryReadArgument(in string text, out int start_i, ref int read_i, out string argument, in string skippables = _empties_, in string stoppers = _stoppers_)
        {
            text.HasNext(ref read_i, skippables: skippables);
            start_i = read_i;

            for (; read_i < text.Length; read_i++)
            {
                char c = text[read_i];
                if (c == ' ' || stoppers != null && stoppers.Contains(c, StringComparison.OrdinalIgnoreCase))
                    break;
            }

            if (read_i > start_i)
            {
                argument = text[start_i..read_i];
                return true;
            }

            argument = null;
            return false;
        }

        public string ReadAll()
        {
            string value = text[read_i..];
            cpl_start = read_i;
            cpl_end = value.Length;
            read_i = value.Length;
            return value;
        }

        public bool TryReadArgument(out string argument, in bool as_function_argument, in Color lint, in string skippables = _empties_, in string stoppers = _stoppers_)
        {
            int read_old = read_i;

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

                    Stderr($"expected ',' or ')' after argument '{argument}'.");
                }

                if (start_i <= cursor_i)
                {
                    cpl_start = read_old + 1;
                    cpl_end = read_i;
                }
            }

            read_i = read_old;
            argument = null;
            return false;

        success:
            LintToThisPosition(lint, true);
            return true;
        }

        public bool TryReadString_match(in string match, in bool as_function_argument, in Color lint, in bool ignore_case = true, in bool add_to_completions = true) => TryReadString_matches_out(out _, as_function_argument, lint: lint, ignore_case: ignore_case, add_to_completions: add_to_completions, matches: new string[] { match, });
        public bool TryReadString_match_out(out string value, in bool as_function_argument, in string match, in Color lint, in bool ignore_case = true, in bool add_to_completions = true) => TryReadString_matches_out(out value, as_function_argument, lint: lint, ignore_case: ignore_case, add_to_completions: add_to_completions, matches: new string[] { match });
        public bool TryReadString_matches_out(out string value, in bool as_function_argument, in Color lint, in IEnumerable<string> matches, in bool strict = true, in bool ignore_case = true, in bool add_to_completions = true, in string skippables = _empties_, in string stoppers = _stoppers_)
        {
            StringComparison ordinal = ignore_case.ToOrdinal();
            int read_old = read_i;

            if (HasNext(ignore_case: ignore_case, skippables: skippables) && TryReadArgument(out value, as_function_argument: as_function_argument, lint: lint, skippables: skippables, stoppers: stoppers))
            {
                if (add_to_completions && !stop_completing)
                    if (IsOnCursor())
                        completions_v.UnionWith(matches);

                if (!strict)
                    return true;

                foreach (string match in matches)
                    if (match.Equals(value, ordinal))
                    {
                        last_arg = value = match;
                        LintToThisPosition(lint, true);

                        if (as_function_argument)
                            if (TryReadChar_match(',', lint: lint_theme.argument_coma))
                                goto success;

                        if (!as_function_argument || !strict_syntax)
                            goto success;

                        if (TryPeekChar_match(')', out _))
                            goto success;

                        goto success;
                    }
            }
            else if (add_to_completions && !stop_completing)
                if (IsOnCursor())
                    completions_v.UnionWith(matches);

            read_i = read_old;
            value = null;
            return false;

        success:
            return true;
        }
    }
}