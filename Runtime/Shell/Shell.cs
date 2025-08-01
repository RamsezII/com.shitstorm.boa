using System;
using System.Collections.Generic;
using _ARK_;
using UnityEngine;

namespace _BOA_
{
    public sealed partial class Shell : MonoBehaviour
    {
        static readonly HashSet<Shell> instances = new();

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
            InitShellHistory();
        }

        //----------------------------------------------------------------------------------------------------------

        private void Awake()
        {
            id = ++_id;
            instances.Add(this);
            AwakeWorkDir();
        }

        //----------------------------------------------------------------------------------------------------------

        private void Start()
        {
            NUCLEOR.delegates.Update_OnShellTick += Tick;
            RefreshShellPrefixe();
        }

        //----------------------------------------------------------------------------------------------------------

        private void OnDestroy()
        {
            NUCLEOR.delegates.Update_OnShellTick -= Tick;
            instances.Remove(this);
            execution?.Dispose();
        }
    }
}