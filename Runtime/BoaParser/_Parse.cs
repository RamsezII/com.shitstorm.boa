using UnityEngine;

namespace _BOA_
{
    partial class BoaParser
    {
        bool TryReadAsInt(in string line, out int value, out string newline)
        {
            if (TryReadToken(line, out string token, out newline))
                if (int.TryParse(token, out value))
                    return true;
            value = 0;
            newline = null;
            return false;
        }

        bool TryReadAsFloat(in string line, out float value, out string newline)
        {
            if (TryReadToken(line, out string token, out newline))
                if (float.TryParse(token, out value))
                    return true;
            value = 0;
            newline = null;
            return false;
        }

        bool TryReadAsVector(in string line, out Vector4 value, out string newline)
        {
            value = Vector4.zero;

            if (TryReadAsFloat(line, out value.x, out newline))
                if (TryReadAsFloat(newline, out value.y, out newline))
                    if (TryReadAsFloat(newline, out value.z, out newline))
                        if (TryReadAsFloat(newline, out value.w, out newline))
                            return true;

            newline = null;
            return false;
        }
    }
}