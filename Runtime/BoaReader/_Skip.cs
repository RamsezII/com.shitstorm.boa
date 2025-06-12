using System;

namespace _BOA_
{
    partial class BoaReader
    {
        public bool SkipUntil(in char expected_value, in string unskippables = null, in bool ignore_case = true)
        {
            while (read_i < text.Length)
            {
                char c = text[read_i];

                if (c == expected_value)
                    return true;

                if (unskippables != null && unskippables.Contains(c, ignore_case ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
                    break;
                else
                    ++read_i;
            }

            return false;
        }
    }
}