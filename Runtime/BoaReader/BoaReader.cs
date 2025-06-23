﻿using System;
using System.Collections.Generic;
using System.IO;

namespace _BOA_
{
    [Serializable]
    public sealed partial class BoaReader
    {
        public readonly bool strict_syntax;
        public readonly string script_path;
        public readonly string[] lines;
        public readonly string text;

        public readonly bool multiline;
        public readonly int first_line, last_line;
        public int cursor_i, cpl_start, read_i, cpl_end;
        public string last_arg;

#if UNITY_EDITOR
        readonly int _text_length;
        string toLog => text[..read_i] + "°" + text[read_i..];
#endif

        public string sig_error, err_trace, sig_long_error;
        public bool IsOnCursor(in int cursor_i) => cursor_i >= cpl_start && cursor_i <= read_i;
        public bool IsOnCursor() => IsOnCursor(cursor_i);

        public readonly HashSet<string> completions_v = new(StringComparer.Ordinal);
        public string completion_l, completion_r;
        internal bool stop_completing;

        //----------------------------------------------------------------------------------------------------------

        public static BoaReader ReadScript(in LintTheme lint_theme, in bool strict_syntax, in string script_path, in int cursor_i = int.MaxValue) => new BoaReader(lint_theme, strict_syntax, script_path, cursor_i, File.ReadAllLines(script_path));
        public static BoaReader ReadLines(in LintTheme lint_theme, in bool strict_syntax, in int cursor_i = int.MaxValue, params string[] lines) => new BoaReader(lint_theme, strict_syntax, "line", cursor_i, lines);
        BoaReader(in LintTheme lint_theme, in bool strict_syntax, in string script_path, in int cursor_i, in string[] lines)
        {
            this.lint_theme = lint_theme ?? LintTheme.theme_dark;
            this.strict_syntax = strict_syntax;
            this.script_path = script_path;
            this.cursor_i = cursor_i;
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
                this.lines = lines[first_line..last_line];
            else
                this.lines = lines;

            multiline = last_line - first_line > 1;
        }

        //----------------------------------------------------------------------------------------------------------

        public bool HasNext(in bool ignore_case = true, in string skippables = _empties_) => text.HasNext(ref read_i, ignore_case: ignore_case, skippables: skippables);

        public void Stderr(in string error)
        {
            sig_error ??= error;
            err_trace = Util.GetStackTrace().GetFrame(1).ToString();
        }
    }
}