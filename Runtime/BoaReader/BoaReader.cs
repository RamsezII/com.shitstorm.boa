using System;
using System.IO;

namespace _BOA_
{
    [Serializable]
    public sealed partial class BoaReader
    {
        public readonly bool strict_syntax;
        public readonly string script_path;
        public readonly string[] source_lines;
        public readonly string text;

        public readonly bool multiline;
        public readonly int first_line, last_line;
        public int start_i, read_i;
        public string last_arg;

#if UNITY_EDITOR
        readonly int _text_length;
        string toLog => text[..read_i] + "°" + text[read_i..];
#endif

        public string error, long_error;

        //----------------------------------------------------------------------------------------------------------

        public static BoaReader ReadScript(in LintTheme lint_theme, in bool strict_syntax, in string script_path) => new BoaReader(lint_theme, strict_syntax, script_path, File.ReadAllLines(script_path));
        public static BoaReader ReadCommandLines(in LintTheme lint_theme, in bool strict_syntax, params string[] command_lines) => new BoaReader(lint_theme, strict_syntax, "line", command_lines);
        BoaReader(in LintTheme lint_theme, in bool strict_syntax, in string script_path, in string[] source_lines)
        {
            this.lint_theme = lint_theme;
            this.strict_syntax = strict_syntax;
            this.script_path = script_path;
            text = source_lines.Join("\n");
#if UNITY_EDITOR
            _text_length = text.Length;
#endif

            first_line = last_line = -1;
            for (int i = 0; i < source_lines.Length; i++)
                if (!string.IsNullOrWhiteSpace(source_lines[i]))
                {
                    if (first_line == -1)
                        first_line = i;
                    last_line = 1 + i;
                }

            if (first_line > 0 || last_line < source_lines.Length)
                this.source_lines = source_lines[first_line..last_line];
            else
                this.source_lines = source_lines;

            multiline = last_line - first_line > 1;
        }

        //----------------------------------------------------------------------------------------------------------

        public bool HasNext(in bool ignore_case = true, in string skippables = _empties_) => text.HasNext(ref read_i, ignore_case: ignore_case, skippables: skippables);
    }
}