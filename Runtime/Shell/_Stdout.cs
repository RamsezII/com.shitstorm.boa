using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace _BOA_
{
    partial class Shell
    {
        readonly Queue<(string, string)> lines = new();
        [SerializeField] bool stdout_flag;
        const byte max_lines = 250;

        //--------------------------------------------------------------------------------------------------------------

        void Stdout(object data) => AddLine(data, null);
        public void AddLine(in object line, in string lint)
        {
            lock (lines)
            {
                while (lines.Count >= max_lines)
                    lines.Dequeue();
                string str = line?.ToString();
                lines.Enqueue((str, lint ?? str));
                stdout_flag = true;
            }
        }

        public bool PullStdout(out string stdout)
        {
            lock (lines)
                if (stdout_flag)
                {
                    stdout_flag = false;
                    StringBuilder sb = new();
                    foreach (object line in lines)
                        sb.AppendLine(line?.ToString());
                    stdout = sb.ToString();
                    return true;
                }
            stdout = null;
            return false;
        }
    }
}