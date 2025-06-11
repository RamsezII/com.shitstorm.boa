using System.Text;

namespace _BOA_
{
    partial class BoaReader
    {
        public string LocalizeError(in string error, in string[] lines)
        {
            StringBuilder sb = new();
            sb.AppendLine();
            sb.AppendLine();

            int char_count = 0;
            for (int i = 0; i < lines.Length; ++i)
            {
                string line = lines[i];

                sb.AppendLine($"{i,5}   {line}");

                if (char_count + line.Length >= read_i)
                    break;

                char_count += line.Length;
            }

            sb.AppendLine($"{new string(' ', start_i - char_count + 9)}^{new string('~', read_i - start_i)}");
            sb.AppendLine();
            sb.AppendLine($"{error}");
            sb.Append($"({nameof(last_arg)}: '{last_arg}')");

            return sb.ToString();
        }
    }
}