namespace _BOA_
{
    partial class BoaReader
    {
        public bool TryPeek(out char value, in bool ignore_case = true, in string skippables = _empties_)
        {
            int read_old = read_i;

            value = default;
            var ordinal = ignore_case.ToOrdinal();

            while (read_i < text.Length)
            {
                value = text[read_i];

                if (skippables != null && skippables.Contains(value, ordinal))
                    ++read_i;
                else
                    return true;
            }

            if (read_i > read_old)
            {
                read_i = read_old;
                return true;
            }

            return false;
        }

        public bool TryRead(out char value, in bool ignore_case = true, in string skippables = _empties_)
        {
            if (TryPeek(out value, ignore_case: ignore_case, skippables: skippables))
            {
                ++read_i;
                return true;
            }
            return false;
        }

        public bool TryPeekSpecific(in char expected_value, in bool ignore_case = true, in string skippables = _empties_)
        {
            int read_old = read_i;
            var ordinal = ignore_case.ToOrdinal();

            while (read_i < text.Length)
            {
                char c = text[read_i];

                if (c == expected_value)
                    return true;

                if (skippables.Contains(c, ordinal))
                    ++read_i;
                else
                    break;
            }

            read_i = read_old;
            return false;
        }

        public bool TryReadMatch(in char expected_value, in bool ignore_case = true, in string skippables = _empties_)
        {
            if (TryPeekSpecific(expected_value, ignore_case: ignore_case, skippables: skippables))
            {
                ++read_i;
                return true;
            }
            return false;
        }

        public bool TryReadMatch(out char value, in bool ignore_case, in string expected_values, in string skippables = _empties_)
        {
            int read_old = read_i;

            if (TryPeek(out value, skippables: skippables) && expected_values.Contains(value, ignore_case.ToOrdinal()))
            {
                ++read_i;
                return true;
            }

            read_i = read_old;
            return false;
        }
    }
}