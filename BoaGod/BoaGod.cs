using _TERMINAL_;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace _BOA_
{
    public class BoaGod : MonoBehaviour, IShell
    {
        public enum Commands : byte
        {
            WriteScript,
            ExecuteScript,
            _last_,
        }

        public static BoaGod instance;

        IEnumerable<string> IShell.ECommands => Enum.GetNames(typeof(Commands));

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            Util.InstantiateOrCreate<BoaGod>();
        }

        //--------------------------------------------------------------------------------------------------------------

        private void Awake()
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Shell.AddUser(instance);
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

                    case Commands.ExecuteScript:
                        OnCmdReadScript(line);
                        break;

                    default:
                        Debug.LogWarning($"{GetType().FullName}.OnCmdLine: \"{code}\" not implemented");
                        break;
                }
            else
                Debug.LogWarning($"{GetType().FullName}.OnCmdLine: \"{arg0}\" not found");
        }

        void OnCmdWriteScript(in LineParser line)
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

        void OnCmdReadScript(in LineParser line)
        {
            string path = line.ReadAsPath();
            if (line.IsCplThis)
                line.CplPath(line, path, out _);
            else if (line.IsExec)
                try
                {
                    FileInfo file = new(path);
                    if (file.Exists)
                        StartCoroutine(new BoaParser().EReadAndExecute(file.FullName, null));
                    else
                        Debug.LogWarning($"File not found: \"{file.FullName}\"");
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e.Message);
                }
        }

        //--------------------------------------------------------------------------------------------------------------

        private void OnDestroy()
        {
            Shell.RemoveUser(instance);
            if (this == instance)
                instance = null;
        }
    }
}