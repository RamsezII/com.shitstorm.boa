using _TERMINAL_;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace _BOA_
{
    internal partial class BoaGod : MonoBehaviour, IShell
    {
        public enum Commands : byte
        {
            ExecuteScript,
            TestPython,
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
                    case Commands.ExecuteScript:
                        OnCmdReadScript(line);
                        break;

                    case Commands.TestPython:
                        OnCmdTestPython(line);
                        break;

                    default:
                        Debug.LogWarning($"{GetType().FullName}.OnCmdLine: \"{code}\" not implemented", this);
                        break;
                }
            else
                Debug.LogWarning($"{GetType().FullName}.OnCmdLine: \"{arg0}\" not found", this);
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