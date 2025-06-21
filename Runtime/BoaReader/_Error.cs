namespace _BOA_
{
    partial class BoaReader
    {
        public void LocalizeError()
        {
            int char_count = 0;
            int eol_lenth = Util.is_windows ? 2 : 1;

            for (int i = 0; i < lines.Length; ++i)
            {
                string line = lines[i];
                if (i == lines.Length - 1 || !string.IsNullOrWhiteSpace(line) && char_count + line.Length >= read_i)
                {
                    int char_i = read_i - char_count + 6;
                    string spaces = new(' ', char_i);

                    if (multiline)
                    {
                        if (false)
                            sig_long_error = $"at {script_path ?? "line"}:{i}\n({nameof(last_arg)}: '{last_arg}', {i}, {char_i})\n {i + ".",-4} {line}\n{spaces}|\n{spaces}└──> {sig_error}";
                        else
                            sig_long_error = $"at {script_path ?? "line"}:{i}\n({nameof(last_arg)}: '{last_arg}', {i}, {char_i})\n {i + ".",-4} {line}\n{spaces}└──> {sig_error}";
                    }
                    else
                        sig_long_error = $"{line}\n{new string(' ', read_i - char_count)}└──> {sig_error}";

                    return;
                }
                char_count += eol_lenth + line.Length;
            }
            sig_long_error = $"\n{sig_error}";
        }
    }
}