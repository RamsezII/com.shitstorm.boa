using System.Collections.Generic;
using _ARK_;
using UnityEngine;

namespace _BOA_
{
    public sealed partial class HarbingerShell
    {
        static byte _id;
        public byte id;
        public override string ToString() => $"{GetType()}[{id}]";

#if UNITY_EDITOR
        string ToLog => ToString();
#endif

        readonly Janitor janitor = new();
        IEnumerator<Contract.Status> execution;

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
        }

        //----------------------------------------------------------------------------------------------------------

        private void Start()
        {
            NUCLEOR.delegates.shell_tick += Tick;
        }

        //----------------------------------------------------------------------------------------------------------

        private void OnDestroy()
        {
            NUCLEOR.delegates.shell_tick -= Tick;
            janitor.Dispose();
            execution?.Dispose();
        }
    }
}