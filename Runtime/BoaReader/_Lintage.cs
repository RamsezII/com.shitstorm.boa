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

        public readonly LintTheme lint_theme;
        readonly List<LintCursor> lint_cursors = new();
        int last_lint_i;
        byte last_braquet;

        //----------------------------------------------------------------------------------------------------------

        public void LintOpeningBraquet() => LintToThisPosition(OpenBraquetLint());
        Color OpenBraquetLint()
        {
            int ind = last_braquet++;
            return GetBraquetLint(ind);
        }

        public void LintClosingBraquet() => LintToThisPosition(CloseBraquetLint());
        public Color CloseBraquetLint()
        {
            int ind = --last_braquet;
            return GetBraquetLint(ind);
        }

        Color GetBraquetLint(in int braquet)
        {
            return (braquet % 3) switch
            {
                0 => lint_theme.bracket_0,
                1 => lint_theme.bracket_1,
                2 => lint_theme.bracket_2,
                _ => LintTheme.lint_default,
            };
        }

        public void LintToThisPosition(in Color color) => LintToThisPosition(color, read_i);
        public void LintToThisPosition(in Color color, in int index)
        {
            if (color.a <= 0)
                return;

            if (index <= last_lint_i)
                UnlintAbovePosition(index);

            lint_cursors.Add(new(index, color));
            last_lint_i = index;
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