using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace _BOA_
{
    partial class Shell
    {
        readonly Queue<(string text, string lint)> lines = new();
        const byte max_lines = 250;
        public string stdout_text, stdout_lint;
        public Action on_stdout;

        //--------------------------------------------------------------------------------------------------------------

        public void AddLine(object data, string lint = null)
        {
            if (data == null)
                return;

            if (data is not string str)
                str = data.ToString();

            if (string.IsNullOrWhiteSpace(str))
                return;

            lint ??= str;

            StringBuilder sb_text = new(), sb_lint = new();

            lock (lines)
            {
                while (lines.Count >= max_lines)
                    lines.Dequeue();

                lines.Enqueue((str, lint));
                Debug.Log($"{ToLog} {lint}", this);

                foreach (var (line_text, line_lint) in lines)
                {
                    sb_text.AppendLine(line_text);
                    sb_lint.AppendLine(line_lint);
                }
            }

            stdout_text = sb_text.ToString();
            stdout_lint = sb_lint.ToString();

            on_stdout?.Invoke();
        }
    }
}