using System.Globalization;
using UnityEngine;

namespace _BOA_
{
    partial class BoaParser
    {
        bool TryComputeFloat(string token, out float value)
        {
            int index;
            token = token.Trim();

            if (token[0] == '(')
            {
                int count = 0;
                for (int i = 0; i < token.Length; i++)
                {
                    char c = token[i];
                    if (c == '(')
                        count++;
                    else if (c == ')')
                        count--;

                    if (count == 0)
                    {
                        string token2 = token[1..i];
                        if (TryComputeFloat(token2, out value))
                            token = value.ToString(CultureInfo.InvariantCulture.NumberFormat) + token[(i + 1)..];
                        break;
                    }
                }
            }

            if ((index = token.IndexOf("+")) > 0)
            {
                string[] tokens = new string[2] { token[..index], token[++index..] };
                if (TryComputeFloat(tokens[0], out float a) && TryComputeFloat(tokens[1], out float b))
                {
                    value = a + b;
                    return true;
                }
            }
            else if ((index = token.IndexOf("-")) > 0)
            {
                string[] tokens = new string[2] { token[..index], token[++index..] };
                if (TryComputeFloat(tokens[0], out float a) && TryComputeFloat(tokens[1], out float b))
                {
                    value = a - b;
                    return true;
                }
            }
            else if ((index = token.LastIndexOf("/")) > 0)
            {
                string[] tokens = new string[2] { token[..index], token[++index..] };
                if (TryComputeFloat(tokens[0], out float a) && TryComputeFloat(tokens[1], out float b))
                {
                    value = a / b;
                    return true;
                }
            }
            else if ((index = token.IndexOf("*")) > 0)
            {
                string[] tokens = new string[2] { token[..index], token[++index..] };
                if (TryComputeFloat(tokens[0], out float a) && TryComputeFloat(tokens[1], out float b))
                {
                    value = a * b;
                    return true;
                }
            }
            else if (float.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out value))
                return true;

            Debug.LogWarning($"{this} failed to compute float: \"{token}\"");
            value = 0;
            return false;
        }

        bool TryComputeInt(string token, out int value)
        {
            int index;
            token = token.Trim();

            if (token[0] == '(')
            {
                int count = 0;
                for (int i = 0; i < token.Length; i++)
                {
                    char c = token[i];
                    if (c == '(')
                        count++;
                    else if (c == ')')
                        count--;

                    if (count == 0)
                    {
                        string token2 = token[1..i];
                        if (TryComputeInt(token2, out value))
                            token = value.ToString(CultureInfo.InvariantCulture.NumberFormat) + token[(i + 1)..];
                        break;
                    }
                }
            }

            if ((index = token.IndexOf("+")) > 0)
            {
                string[] tokens = new string[2] { token[..index], token[++index..] };
                if (TryComputeInt(tokens[0], out int a) && TryComputeInt(tokens[1], out int b))
                {
                    value = a + b;
                    return true;
                }
            }
            else if ((index = token.IndexOf("-")) > 0)
            {
                string[] tokens = new string[2] { token[..index], token[++index..] };
                if (TryComputeInt(tokens[0], out int a) && TryComputeInt(tokens[1], out int b))
                {
                    value = a - b;
                    return true;
                }
            }
            else if ((index = token.LastIndexOf("/")) > 0)
            {
                string[] tokens = new string[2] { token[..index], token[++index..] };
                if (TryComputeInt(tokens[0], out int a) && TryComputeInt(tokens[1], out int b))
                {
                    value = a / b;
                    return true;
                }
            }
            else if ((index = token.IndexOf("*")) > 0)
            {
                string[] tokens = new string[2] { token[..index], token[++index..] };
                if (TryComputeInt(tokens[0], out int a) && TryComputeInt(tokens[1], out int b))
                {
                    value = a * b;
                    return true;
                }
            }
            else if (float.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out float f))
            {
                value = (int)f;
                return true;
            }

            Debug.LogWarning($"{this} failed to compute float: \"{token}\"");
            value = 0;
            return false;
        }
    }
}