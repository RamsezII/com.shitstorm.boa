using System;

namespace _BOA_
{
    partial class BoaReader
    {
        public static bool TryReadArgument(in string text, out int start_i, ref int read_i, out string argument, in string blacklist = blacklist_boa)
        {
            text.HasNext(ref read_i);
            start_i = read_i;

            for (; read_i < text.Length; read_i++)
            {
                char c = text[read_i];
                if (c == ' ' || blacklist != null && blacklist.Contains(c, StringComparison.OrdinalIgnoreCase))
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
    }
}