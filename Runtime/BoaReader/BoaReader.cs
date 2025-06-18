using System;
using System.Collections.Generic;
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
        public int write_i, start_i, read_i;
        public string last_arg;

#if UNITY_EDITOR
        readonly int _text_length;
        string toLog => text[..read_i] + "°" + text[read_i..];
#endif

        public string error, long_error;

        internal readonly HashSet<string> completions = new(StringComparer.Ordinal);

        //----------------------------------------------------------------------------------------------------------

        public static BoaReader ReadScript(in LintTheme lint_theme, in bool strict_syntax, in string script_path, in int write_i = int.MaxValue) => new BoaReader(lint_theme, strict_syntax, script_path, write_i, File.ReadAllLines(script_path));
        public static BoaReader ReadLines(in LintTheme lint_theme, in bool strict_syntax, in int write_i = int.MaxValue, params string[] lines) => new BoaReader(lint_theme, strict_syntax, "line", write_i, lines);
        BoaReader(in LintTheme lint_theme, in bool strict_syntax, in string script_path, in int write_i, in string[] lines)
        {
            this.lint_theme = lint_theme;
            this.strict_syntax = strict_syntax;
            this.script_path = script_path;
            this.write_i = write_i;
            text = lines.Join("\n");
#if UNITY_EDITOR
            _text_length = text.Length;
#endif

            first_line = last_line = -1;
            for (int i = 0; i < lines.Length; i++)
                if (!string.IsNullOrWhiteSpace(lines[i]))
                {
                    if (first_line == -1)
                        first_line = i;
                    last_line = 1 + i;
                }

            if (first_line > 0 || last_line < lines.Length)
                this.source_lines = lines[first_line..last_line];
            else
                this.source_lines = lines;

            multiline = last_line - first_line > 1;
        }

        //----------------------------------------------------------------------------------------------------------

        public bool HasNext(in bool ignore_case = true, in string skippables = _empties_) => text.HasNext(ref read_i, ignore_case: ignore_case, skippables: skippables);
    }
}