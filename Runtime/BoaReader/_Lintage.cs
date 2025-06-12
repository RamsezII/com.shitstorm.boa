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

        public void AddCursor(in int index, in Color color)
        {
            for (int i = lint_cursors.Count - 1; i >= 0; i--)
                if (lint_cursors[i].index >= index)
                    lint_cursors.RemoveAt(i);
            lint_cursors.Add(new(index, color));
        }

        public string GetLint(in Color default_color)
        {
            StringBuilder sb = new();

            for (int i = 1; i < lint_cursors.Count; ++i)
                sb.Append(text[lint_cursors[i - 1].index..lint_cursors[i].index].SetColor(lint_cursors[i].color));

            if (lint_cursors[^1].index < text.Length)
                switch (lint_cursors.Count)
                {
                    case 0:
                        sb.Append(text.SetColor(default_color));
                        break;

                    case 1:
                        sb.Append(text[lint_cursors[^1].index..].SetColor(default_color));
                        break;

                    default:
                        sb.Append(text[lint_cursors[^2].index..lint_cursors[^1].index].SetColor(default_color));
                        break;
                }

            return sb.ToString();
        }
    }
}