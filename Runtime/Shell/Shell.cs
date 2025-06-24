using System;
using _ARK_;
using UnityEngine;

namespace _BOA_
{
    public sealed partial class Shell : MonoBehaviour
    {
        static byte _id;
        public byte id;
        public override string ToString() => $"{GetType()}[{id}]";

#if UNITY_EDITOR
        string ToLog => ToString();
#endif
        public Action<string, string> stdout;

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            _id = 0;
        }

        //----------------------------------------------------------------------------------------------------------

        private void Awake()
        {
            id = ++_id;
            AwakeWorkDir();
        }

        //----------------------------------------------------------------------------------------------------------

        private void Start()
        {
            NUCLEOR.delegates.shell_tick += Tick;
            RefreshShellPrefixe();
        }

        //----------------------------------------------------------------------------------------------------------

        private void OnDestroy()
        {
            NUCLEOR.delegates.shell_tick -= Tick;
            execution?.Dispose();
        }
    }
}