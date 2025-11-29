using _COBRA_;
using System;
using System.Collections.Generic;

namespace _BOA_
{
    [Serializable]
    public sealed partial class BoaReader
    {
        public readonly bool strict_syntax;
        public readonly string script_path;
        public readonly string text;

        public readonly bool multiline;
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

        public BoaReader(in LintTheme lint_theme, in bool strict_syntax, in string text, in string script_path, in int cursor_i = int.MaxValue)
        {
            this.lint_theme = lint_theme ?? LintTheme.theme_dark;
            this.strict_syntax = strict_syntax;
            this.script_path = script_path;
            this.cursor_i = cursor_i;
            this.text = text;
#if UNITY_EDITOR
            _text_length = text.Length;
#endif
            if (Util.TryIndexOf_min(text, out int index_of, 0, true, '\n', '\r'))
                multiline = index_of < text.Length - 1;
        }

        //----------------------------------------------------------------------------------------------------------

        public bool HasNext(in bool ignore_case = true, in string skippables = _empties_) => text.HasNext(ref read_i, ignore_case: ignore_case, skippables: skippables);

        public void Stderr(in string error)
        {
            sig_error ??= error;
            err_trace ??= Util.GetStackTrace().GetFrame(1).ToString();
        }
    }
}