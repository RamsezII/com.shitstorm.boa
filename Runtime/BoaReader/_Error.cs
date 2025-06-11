using UnityEngine;

namespace _BOA_
{
    partial class BoaReader
    {
        public string LocalizeError(in string error, in string[] lines)
        {
            int char_count = 0;
            int error_i = Mathf.Min(start_i, read_i);

            for (int i = 0; i < lines.Length; ++i)
            {
                string line = lines[i];
                if (!string.IsNullOrWhiteSpace(line))
                    if (char_count + line.Length >= error_i)
                    {
                        int char_i = error_i - char_count + 6;
                        string spaces = new(' ', char_i);
                        return $"({nameof(last_arg)}: '{last_arg}', {i}, {char_i})\n {i + ".",-4} {line}\n{spaces}|\n{spaces}└──> {error}";
                    }
                char_count += 1 + line.Length;
            }
            return error;
        }
    }
}