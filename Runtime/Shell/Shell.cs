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
        public Action<string> change_stdin;

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
            ArkMachine.UserListener(() =>
            {
                ReadHistory(true);
                NUCLEOR.delegates.onApplicationFocus += OnFocus;
                NUCLEOR.delegates.onApplicationUnfocus += OnUnfocus;
            });
        }

        //----------------------------------------------------------------------------------------------------------

        void OnFocus() => ReadHistory(false);
        void OnUnfocus() => WriteHistory(false);

        //----------------------------------------------------------------------------------------------------------

        private void OnDestroy()
        {
            NUCLEOR.delegates.shell_tick -= Tick;
            NUCLEOR.delegates.onApplicationFocus -= OnFocus;
            NUCLEOR.delegates.onApplicationUnfocus -= OnUnfocus;
            execution?.Dispose();
        }
    }
}