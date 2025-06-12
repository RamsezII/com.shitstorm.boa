using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        public bool TryParseString(in BoaReader reader, out string value, out string error)
        {
            int read_old = reader.read_i;

            error = null;

            if (reader.HasNext())
            {
                char sep = default;

                if (reader.TryReadMatch('\''))
                    sep = '\'';
                else if (reader.TryReadMatch('"'))
                    sep = '"';

                if (sep != default)
                {
                    value = string.Empty;

                    while (reader.TryRead(out char c, skippables: null))
                        switch (c)
                        {
                            case '\\':
                                ++reader.read_i;
                                break;

                            case '\'' or '"' when c == sep:
                                reader.last_arg = value;
                                return true;

                            default:
                                value += c;
                                break;
                        }

                    if (value.TryIndexOf_min(out int err_index, true, ' ', '\t', '\n', '\r'))
                        value = value[..err_index];
                    error ??= $"uncomplete string: expected closing quote '{sep}' after {{ {sep}{value} }}";
                }
            }

            value = null;
            reader.read_i = read_old;
            return false;
        }
    }
}