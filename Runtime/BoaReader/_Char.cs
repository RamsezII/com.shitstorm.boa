using System.Linq;
using UnityEngine;

namespace _BOA_
{
    partial class BoaReader
    {
        public bool TryPeekChar_out(out char value, out int next_i, in bool ignore_case = true, in string skippables = _empties_)
        {
            value = default;
            int read_old = read_i;
            var ordinal = ignore_case.ToOrdinal();

            if (read_i < text.Length)
            {
                while (read_i < text.Length)
                {
                    value = text[read_i];

                    if (skippables != null && skippables.Contains(value, ordinal))
                        ++read_i;
                    else
                    {
                        cpl_start = read_i;
                        cpl_end = 1 + read_i;
                        next_i = read_i;
                        return true;
                    }
                }
                cpl_start = Mathf.Min(read_old + 1, read_i);
                cpl_end = read_i;
            }

        failure:
            next_i = read_i;
            read_i = read_old;
            return false;
        }

        public bool TryReadChar_out(out char value, in bool ignore_case = true, in string skippables = _empties_)
        {
            if (TryPeekChar_out(out value, out _, ignore_case: ignore_case, skippables: skippables))
            {
                ++read_i;
                last_arg = value.ToString();
                return true;
            }
            return false;
        }

        public bool TryPeekChar_match(in char expected_value, out int next_i, in bool ignore_case = true, in bool add_to_completions = false, in string skippables = _empties_)
        {
            int read_old = read_i;
            var ordinal = ignore_case.ToOrdinal();

            if (read_i < text.Length)
            {
                while (read_i < text.Length)
                {
                    char c = text[read_i];

                    if (c == expected_value)
                    {
                        next_i = read_i;

                        if (read_old <= cursor_i)
                        {
                            cpl_start = read_i;
                            cpl_end = 1 + read_i;

                            if (add_to_completions && !stop_completing)
                                completions_v.Add(expected_value.ToString());
                        }

                        return true;
                    }

                    if (skippables != null && skippables.Contains(c, ordinal))
                        ++read_i;
                    else
                        break;
                }

                if (read_old <= cursor_i && cursor_i <= read_i)
                {
                    cpl_start = Mathf.Min(read_old + 1, read_i);
                    cpl_end = read_i;

                    if (add_to_completions && !stop_completing)
                        completions_v.Add(expected_value.ToString());
                }
            }

            next_i = read_i;
            read_i = read_old;
            return false;
        }

        public bool TryReadChar_match(in char expected_value, in Color lint = default, in bool add_to_completions = false, in bool ignore_case = true, in string skippables = _empties_)
        {
            if (TryPeekChar_match(expected_value, out _, add_to_completions: add_to_completions, ignore_case: ignore_case, skippables: skippables))
            {
                ++read_i;
                LintToThisPosition(lint, true);
                last_arg = expected_value.ToString();
                return true;
            }
            return false;
        }

        public bool TryReadChar_matches_out(out char value, in bool ignore_case, in string expected_values, in bool add_to_completions = false, in string skippables = _empties_)
        {
            int read_old = read_i;

            if (TryPeekChar_out(out value, out int next_i, skippables: skippables) && expected_values.Contains(value, ignore_case.ToOrdinal()))
            {
                ++read_i;

                if (add_to_completions && !stop_completing)
                    if (IsOnCursor(next_i))
                        completions_v.UnionWith(expected_values.Select(c => c.ToString()));

                last_arg = value.ToString();
                return true;
            }

            if (add_to_completions && !stop_completing)
                if (IsOnCursor(next_i))
                    completions_v.UnionWith(expected_values.Select(c => c.ToString()));

            read_i = read_old;
            return false;
        }
    }
}