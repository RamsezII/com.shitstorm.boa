using System.Collections.Generic;
using System.Text;
using _ARK_;
using UnityEngine;

namespace _BOA_
{
    partial class Shell
    {
        readonly Queue<object> lines = new();
        [SerializeField] bool stdout_flag;
        const byte max_lines = 250;

        //--------------------------------------------------------------------------------------------------------------

        void AddLine_log(LogManager.LogInfos log)
        {
            switch (log.type)
            {
                case LogType.Exception:
                case LogType.Assert:
                    AddLine(log.message, $"<color=red>{log.message}</color>");
                    break;

                case LogType.Error:
                    AddLine(log.message, $"<color=orange>{log.message}</color>");
                    break;

                case LogType.Warning:
                    AddLine(log.message, $"<color=yellow>{log.message}</color>");
                    break;

                default:
                    AddLine(log.message, log.message);
                    break;
            }
        }

        public void AddLine(in object line, in string lint)
        {
            lock (lines)
            {
                while (lines.Count >= max_lines)
                    lines.Dequeue();
                lines.Enqueue(lint);
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