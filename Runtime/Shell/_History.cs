using _ARK_;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace _BOA_
{
    partial class Shell
    {
        [Serializable]
        class History : UserJSon
        {
            public string[] lines;
        }

        readonly List<string> history = new(history_max);

        const byte history_max = 50;
        [SerializeField] int history_index = -1;

        //--------------------------------------------------------------------------------------------------------------

        void WriteHistory(in bool log)
        {
            var saved_history = new History
            {
                lines = history.ToArray()
            };
            saved_history.SaveStaticJSon(log);
        }

        void ReadHistory(in bool log)
        {
            history.Clear();
            History saved_history = null;
            if (StaticJSon.ReadStaticJSon(ref saved_history, true, log))
                history.AddRange(saved_history.lines[..Mathf.Min(history_max, saved_history.lines.Length)]);
        }

        void AddToHistory(in string line)
        {
            if (history.Contains(line))
                history.Remove(line);
            else if (history.Count >= history_max - 1)
                history.RemoveAt(0);

            history.Add(line);

            ResetHistoryNav();
        }

        public void ResetHistoryNav() => history_index = history.Count;

        public bool TryNavHistory(in int increment, out string value)
        {
            if (history.Count == 0)
            {
                history_index = -1;
                value = null;
                return false;
            }

            int count_mod = 1 + history.Count;

            history_index += increment;

            history_index %= count_mod;
            if (history_index < 0)
                history_index += count_mod;

            if (history_index == history.Count)
                value = string.Empty;
            else
                value = history[history_index];

            return true;
        }
    }
}