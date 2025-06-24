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

        public void LintOpeningBraquet() => LintToThisPosition(OpenBraquetLint(), true);
        Color OpenBraquetLint()
        {
            int ind = last_braquet++;
            return GetBraquetLint(ind);
        }

        public void LintClosingBraquet() => LintToThisPosition(CloseBraquetLint(), true);
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
                _ => lint_theme.fallback_default,
            };
        }

        public bool Remains() => Remains(read_i);
        public bool Remains(in int index)
        {
            if (lint_cursors.Count == 0)
                return false;
            return lint_cursors[^1].index < index;
        }

        public void LintToThisPosition(in Color color, in bool replace) => LintToThisPosition(color, replace, read_i);
        public void LintToThisPosition(in Color color, in bool replace, in int index)
        {
            if (color.a <= 0)
                return;

            if (replace)
            {
                if (index <= last_lint_i)
                    UnlintAbovePosition(index);
                lint_cursors.Add(new(index, color));
            }
            else if (Remains(index))
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

        public string GetLintResult(in int start = 0)
        {
            if (lint_cursors.Count == 0)
                return text[start..].SetColor(lint_theme.fallback_default);

            StringBuilder sb = new();
            int last_lint = start;

            for (int i = 0; i < lint_cursors.Count; ++i)
                if (lint_cursors[i].index >= start)
                {
                    LintCursor cursor = lint_cursors[i];
                    if (cursor.index > last_lint && cursor.index <= text.Length)
                        sb.Append(text[last_lint..cursor.index].SetColor(cursor.color));
                    last_lint = cursor.index;
                }

            if (last_lint < text.Length)
                sb.Append(text[last_lint..].SetColor(lint_theme.fallback_default));

            return sb.ToString();
        }
    }
}