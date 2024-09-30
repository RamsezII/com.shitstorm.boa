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
            line = line.Trim();

            if (string.IsNullOrWhiteSpace(line))
            {
                token = null;
                newline = string.Empty;
                return false;
            }

            if (line[0] == '$')
            {
                line = line[1..];
                if (TryReadAsInt(line, out int i_arg, out newline))
                    if (i_arg < args.Length)
                    {
                        token = args[i_arg];
                        return true;
                    }
                    else
                    {
                        token = string.Empty;
                        return false;
                    }
            }

            if (line.TryReadWord(out string varname, out newline))
                if (variables.TryGetValue(varname, out token))
                    return true;

            switch (line[0])
            {
                case '"':
                case '\'':
                    {
                        int i = line.IndexOf(line[0], 1);
                        if (i > 0)
                        {
                            token = line[1..i];
                            newline = line[(i + 1)..];
                            return true;
                        }
                        token = line[1..];
                        newline = string.Empty;
                    }
                    return true;

                case '(':
                    if (line[0] == '(')
                    {
                        int count = 0;
                        for (int i = 0; i < line.Length; i++)
                        {
                            if (line[i] == '(')
                                count++;
                            else if (line[i] == ')')
                                count--;

                            if (count == 0)
                            {
                                token = line[1..i].Trim();
                                newline = line[(i + 1)..].Trim();
                                return true;
                            }
                        }
                    }
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