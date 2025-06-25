using System.Collections.Generic;
using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        public bool TryParseString(in BoaReader reader, in ScopeNode scope, out ExpressionExecutor executor)
        {
            executor = null;
            int read_old = reader.read_i;

            char sep = default;

            if (reader.TryReadChar_match('\'', lint: reader.lint_theme.quotes))
                sep = '\'';
            else if (reader.TryReadChar_match('"', lint: reader.lint_theme.quotes))
                sep = '"';

            if (sep == default)
                return false;

            reader.cpl_start = Mathf.Min(read_old + 1, reader.read_i - 1);
            reader.cpl_end = reader.read_i - 1;

            List<Executor> stack = new();
            string value = string.Empty;
            int start_i = reader.read_i;

            while (reader.TryReadChar_out(out char c, skippables: null))
                switch (c)
                {
                    case '\\':
                        reader.LintToThisPosition(reader.lint_theme.strings, false, reader.read_i - 1);
                        ++reader.read_i;
                        reader.LintToThisPosition(reader.lint_theme.quotes, false);
                        break;

                    case '\'' or '"' when c == sep:
                        {
                            reader.LintToThisPosition(reader.lint_theme.strings, false, reader.read_i - 1);
                            reader.LintToThisPosition(reader.lint_theme.quotes, false);

                            reader.last_arg = value;
                            reader.cpl_end = reader.read_i - 1;

                            if (value.Length > 0)
                                stack.Add(new LiteralExecutor(this, scope, value));

                            if (stack.Count > 0)
                                executor = new StringExecutor(this, scope, stack);
                        }
                        return true;

                    case '{':
                        {
                            reader.LintToThisPosition(reader.lint_theme.strings, false, reader.read_i - 1);
                            reader.LintToThisPosition(reader.lint_theme.quotes, false);

                            if (value.Length > 0)
                            {
                                stack.Add(new LiteralExecutor(this, scope, value));
                                value = string.Empty;
                            }

                            if (!TryParseExpression(reader, scope, false, out var expr))
                            {
                                reader.Stderr($"expected expression after '{{'.");
                                return false;
                            }

                            if (!reader.TryReadChar_match('}'))
                            {
                                reader.Stderr($"expected closing braquet '}}'.");
                                return false;
                            }

                            reader.LintToThisPosition(reader.lint_theme.strings, false, reader.read_i - 1);
                            reader.LintToThisPosition(reader.lint_theme.quotes, false);

                            stack.Add(expr);
                        }
                        break;

                    default:
                        value += c;
                        break;
                }

            if (value.TryIndexOf_min(out int err_index, 0, true, ' ', '\t', '\n', '\r'))
                reader.read_i = start_i + err_index;
            else
                reader.read_i = read_old;

            reader.LintToThisPosition(reader.lint_theme.quotes, false);

            reader.Stderr($"string error: expected closing quote '{sep}'.");
            return false;
        }
    }
}