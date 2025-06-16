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

        //----------------------------------------------------------------------------------------------------------

        public void LintToThisPosition(in int index, in Color color)
        {
            UnlintAbovePosition(index);
            lint_cursors.Add(new(index, color));
        }

        public void UnlintAbovePosition(in int index)
        {
            for (int i = lint_cursors.Count - 1; i >= 0; i--)
                if (lint_cursors[i].index >= index)
                    lint_cursors.RemoveAt(i);
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