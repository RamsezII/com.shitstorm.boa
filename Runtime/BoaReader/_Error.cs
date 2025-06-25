using System.Text;

namespace _BOA_
{
    partial class BoaReader
    {
        public void LocalizeError()
        {
            if (sig_long_error != null)
                return;

            int line_count = 0;
            int column_count = 0;
            int start_line = 0;
            string line = null;

            if (!multiline)
            {
                column_count = read_i;
                line = text;
            }
            else
                for (int i = 0; i < read_i; ++i)
                    switch (text[i])
                    {
                        case '\n':
                            ++line_count;
                            column_count = 0;
                            break;

                        case '\r':
                            ++line_count;
                            ++i;
                            column_count = 0;
                            break;

                        default:
                            if (column_count == 0)
                                start_line = i;
                            ++column_count;
                            break;
                    }

            if (multiline)
            {
                if (text.TryIndexOf_min(out int next_rn, start_line, true, '\r', '\n'))
                    line = text[start_line..next_rn];
                else
                    line = text[start_line..];

                StringBuilder sb = new();

                sb.AppendLine($"at {script_path ?? "line"}:{line_count}");
                sb.AppendLine($"({nameof(last_arg)}: '{last_arg}', {line_count}, {column_count})");
                sb.AppendLine($" {line_count + ".",-4} {line}");
                sb.Append($"{new string(' ', 6 + column_count)}└──> {sig_error}");

                sig_long_error = sb.ToString();
            }
            else
                sig_long_error = $"{line}\n{new string(' ', read_i)}└──> {sig_error}";
            sig_long_error += "\n\n" + err_trace;
        }
    }
}