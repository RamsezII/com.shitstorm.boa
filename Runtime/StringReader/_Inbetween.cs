using System;

namespace _BOA_
{
    partial class BoaReader
    {
        public bool TryReadBetween(in char open, in char close, in bool strict, in Action read)
        {
            if (!text.HasNext(ref read_i))
                return false;

            char c = text[read_i];
            bool positive = c == open;

            if (positive)
                ++read_i;
            else if (strict)
                return false;

            read();

            if (!strict && !positive)
                return true;

            if (text.HasNext(ref read_i) && text[read_i] == close)
            {
                ++read_i;
                return true;
            }

            return false;
        }
    }
}