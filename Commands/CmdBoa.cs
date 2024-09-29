using _TERMINAL_;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace _BOA_
{
    internal class CmdBoa : IShell
    {
        public enum Commands : byte
        {
            WriteScript,
            ReadScript,
            _last_,
        }

        IEnumerable<string> IShell.ECommands => Enum.GetNames(typeof(Commands));

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            Shell.AddUser(new CmdBoa());
        }

        //--------------------------------------------------------------------------------------------------------------

        void IShell.OnCmdLine(in string arg0, in LineParser line)
        {
            if (Enum.TryParse(arg0, true, out Commands code) && code < Commands._last_)
                switch (code)
                {
                    case Commands.WriteScript:
                        OnCmdWriteScript(line);
                        break;

                    case Commands.ReadScript:
                        OnCmdReadScript(line);
                        break;

                    default:
                        Debug.LogWarning($"{typeof(CmdBoa).FullName}.OnCmdLine: \"{code}\" not implemented");
                        break;
                }
            else
                Debug.LogWarning($"{typeof(CmdBoa).FullName}.OnCmdLine: \"{arg0}\" not found");
        }

        static void OnCmdWriteScript(in LineParser line)
        {
            string path = line.ReadAsPath();
            if (line.IsExec)
                try
                {
                    FileInfo file = new(path);
                    if (!File.Exists(file.FullName))
                    {
                        Debug.Log($"Creating file: \"{file.FullName}\"");
                        File.WriteAllText(file.FullName, string.Empty);
                    }
                    Debug.Log($"Opening file: \"{file.FullName}\"");
                    Application.OpenURL(file.FullName);
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e.Message);
                }
        }

        static void OnCmdReadScript(in LineParser line)
        {
            string path = line.ReadAsPath();
            if (line.IsCplThis)
                line.CplPath(line, path, out _);
            else if (line.IsExec)
                try
                {
                    FileInfo file = new(path);
                    if (file.Exists)
                        BoaParser.Execute(file.FullName);
                    else
                        Debug.LogWarning($"File not found: \"{file.FullName}\"");
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e.Message);
                }
        }
    }
}