using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace _BOA_
{
    partial class BoaReader
    {
        readonly struct LintCursor
        {
            internal readonly int index;
            internal readonly Color color;

            //----------------------------------------------------------------------------------------------------------

            internal LintCursor(in int index, in Color color)
            {
                this.index = index;
                this.color = color;
            }
        }

        readonly List<LintCursor> lint_cursors = new();
        int last_lint_i;

        //----------------------------------------------------------------------------------------------------------

        public void LintToThisPosition(in Color color)
        {
            if (read_i <= last_lint_i)
                UnlintAbovePosition(read_i);
            lint_cursors.Add(new(read_i, color));
            last_lint_i = read_i;
        }

        public void UnlintAbovePosition(in int index)
        {
            for (int i = 0; i < lint_cursors.Count; i++)
                if (lint_cursors[i].index >= index)
                {
                    lint_cursors.RemoveRange(i, lint_cursors.Count - i);
                    break;
                }
        }

        public string GetLintResult(in Color default_color, in int start = 0)
        {
            if (lint_cursors.Count == 0)
                return text[start..].SetColor(default_color);

            StringBuilder sb = new();
            int last_lint = start;

            for (int i = 0; i < lint_cursors.Count; ++i)
                if (lint_cursors[i].index >= start)
                {
                    LintCursor cursor = lint_cursors[i];
                    int index_left = last_lint;
                    sb.Append(text[index_left..cursor.index].SetColor(cursor.color));
                    last_lint = cursor.index;
                }

            if (last_lint < text.Length)
                sb.Append(text[last_lint..].SetColor(default_color));

            return sb.ToString();
        }
    }
}