using System.Collections.Generic;
using System;

namespace _BOA_
{
    partial class BoaParser
    {
        readonly Dictionary<string, string> variables = new(StringComparer.Ordinal);

        //--------------------------------------------------------------------------------------------------------------

        bool TryDeclareVariable(in string line, out string newline)
        {
            if (line.TryReadWord(out string name, out newline))
                if (TryReadToken(newline, out string token, out newline))
                {
                    variables[name] = token;
                    return true;
                }
            return false;
        }

        bool TryReadToken(string line, out string token, out string newline)
        {
            if (line.TryReadWord(out string varname, out newline))
                if (variables.TryGetValue(varname, out token))
                    return true;

            line = line.Trim();

            bool TryParse(in char separator, out string token, out string newline)
            {
                int i = line.IndexOf(separator, 1);
                if (i > 0)
                {
                    token = line[1..i];
                    newline = line[(i + 1)..].Trim();
                    return true;
                }
                token = null;
                newline = string.Empty;
                return false;
            }

            switch (line[0])
            {
                case '"':
                case '\'':
                    if (TryParse(line[0], out token, out newline))
                        return true;
                    break;

                case '{':
                    if (TryParse('}', out token, out newline))
                        return true;
                    break;

                case '(':
                    if (TryParse(')', out token, out newline))
                        return true;
                    break;

                case '[':
                    if (TryParse(']', out token, out newline))
                        return true;
                    break;

                default:
                    if (line.TryReadWord(out token, out newline))
                        return true;
                    break;
            }

            token = null;
            newline = string.Empty;
            return false;
        }
    }
}