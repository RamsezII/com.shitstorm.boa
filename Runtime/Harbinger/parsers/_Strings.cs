using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        public bool TryParseString(in BoaReader reader, in ScopeNode scope, out string value)
        {
            int read_old = reader.read_i;
            reader.sig_error = null;

            if (reader.HasNext())
            {
                reader.cpl_start = Mathf.Min(read_old + 1, reader.read_i);
                reader.cpl_end = reader.read_i;

                char sep = default;

                if (reader.TryReadChar_match('\'', lint: reader.lint_theme.quotes))
                    sep = '\'';
                else if (reader.TryReadChar_match('"', lint: reader.lint_theme.quotes))
                    sep = '"';

                if (sep != default)
                {
                    value = string.Empty;
                    int start_i = reader.read_i;
                    reader.LintToThisPosition(reader.lint_theme.quotes);

                    while (reader.TryReadChar_out(out char c, skippables: null))
                        switch (c)
                        {
                            case '\\':
                                ++reader.read_i;
                                break;

                            case '\'' or '"' when c == sep:
                                reader.LintToThisPosition(reader.lint_theme.strings, reader.read_i - 1);
                                reader.LintToThisPosition(reader.lint_theme.quotes);
                                reader.last_arg = value;
                                return true;

                            case '{':
                                if (!TryParseExpression(reader, scope, false, out var expr))
                                {
                                    reader.sig_error ??= $"expected expression";
                                    value = null;
                                    return false;
                                }
                                if (!reader.TryReadChar_match('}'))
                                {
                                    reader.sig_error ??= $"expected closing braquet '}}'.";
                                    value = null;
                                    return false;
                                }
                                break;

                            default:
                                value += c;
                                break;
                        }

                    if (value.TryIndexOf_min(out int err_index, true, ' ', '\t', '\n', '\r'))
                    {
                        value = value[..err_index];
                        reader.read_i = start_i + err_index;
                    }

                    reader.sig_error ??= $"string error: expected closing quote '{sep}'";
                    return false;
                }
            }

            value = null;
            reader.read_i = read_old;
            return false;
        }
    }
}