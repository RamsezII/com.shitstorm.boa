using System;
using System.Collections.Generic;
using UnityEngine;

namespace _BOA_
{
    internal sealed class Janitor : IDisposable
    {
        static byte _id;
        public byte id;
        public override string ToString() => $"janitor[{id}]";

#if UNITY_EDITOR
        string ToLog => ToString();
#endif

        readonly List<(Executor exe, IEnumerator<Contract.Status> routine)> stack = new();

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            _id = 0;
        }

        //----------------------------------------------------------------------------------------------------------

        public Janitor()
        {
            id = ++_id;
        }

        //----------------------------------------------------------------------------------------------------------

        public void Dispose()
        {
            for (int i = 0; i < stack.Count; ++i)
            {
                var (exe, routine) = stack[i];
                exe.Dispose();
                routine.Dispose();
            }
            stack.Clear();
        }
    }
}