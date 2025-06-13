namespace _BOA_
{
    partial class BoaReader
    {
        public bool TryPeekChar_out(out char value, in bool ignore_case = true, in string skippables = _empties_)
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

        public bool TryReadChar_out(out char value, in bool ignore_case = true, in string skippables = _empties_)
        {
            if (TryPeekChar_out(out value, ignore_case: ignore_case, skippables: skippables))
            {
                ++read_i;
                return true;
            }
            return false;
        }

        public bool TryPeekChar_match(in char expected_value, in bool ignore_case = true, in string skippables = _empties_)
        {
            int read_old = read_i;
            var ordinal = ignore_case.ToOrdinal();

            while (read_i < text.Length)
            {
                char c = text[read_i];

                if (c == expected_value)
                    return true;

                if (skippables != null && skippables.Contains(c, ordinal))
                    ++read_i;
                else
                    break;
            }

            read_i = read_old;
            return false;
        }

        public bool TryReadChar_match(in char expected_value, in bool ignore_case = true, in string skippables = _empties_)
        {
            if (TryPeekChar_match(expected_value, ignore_case: ignore_case, skippables: skippables))
            {
                ++read_i;
                return true;
            }
            return false;
        }

        public bool TryReadChar_match_out(out char value, in bool ignore_case, in string expected_values, in string skippables = _empties_)
        {
            int read_old = read_i;

            if (TryPeekChar_out(out value, skippables: skippables) && expected_values.Contains(value, ignore_case.ToOrdinal()))
            {
                ++read_i;
                return true;
            }

            read_i = read_old;
            return false;
        }
    }
}