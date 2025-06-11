namespace _BOA_
{
    partial class BoaReader
    {
        public string LocalizeError(in string error, in string[] lines)
        {
            int char_count = 0;
            for (int i = 0; i < lines.Length; ++i)
            {
                string line = lines[i];
                if (char_count + line.Length < read_i)
                    char_count += line.Length;
                else
                {
                    int char_i = start_i - char_count + 7;
                    string spaces = new(' ', char_i);
                    return $"({nameof(last_arg)}: '{last_arg}', {i}, {char_i})\n {i + ".",-4} {line}\n{spaces}|\n{spaces}└──> {error}";
                }
            }
            return error;
        }
    }
}