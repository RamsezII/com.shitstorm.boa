using System;
using System.Collections.Generic;
using UnityEngine;

namespace _BOA_
{
    public sealed class Contract
    {
        [Serializable]
        public struct Status
        {
            public enum States
            {
                BLOCKING,
                WAIT_FOR_STDIN,
                BACKGROUND,
                DAEMON,
            }

            public States state;
            public string prefixe;
            [Range(0, 1)] public float progress;
            public object data;
        }

        public readonly string name;
        internal readonly Type type;
        internal readonly int min_args, max_args;
        public readonly Action<ContractExecutor> args;
        internal readonly Func<ContractExecutor, object> action;
        internal readonly Func<ContractExecutor, IEnumerator<Status>> routine;

        //----------------------------------------------------------------------------------------------------------

        public Contract(in string name, 
            in Type type = null,
            in int min_args = 0,
            in int max_args = 0,
            in Action<ContractExecutor> args = null,
            in Func<ContractExecutor, object> action = null,
            in Func<ContractExecutor, IEnumerator<Status>> routine = null
            )
        {
            this.name = name;
            this.type = type;
            this.min_args = min_args;
            this.max_args = Mathf.Max(min_args, max_args);
            this.args = args;
            this.action = action;
            this.routine = routine;
        }
    }
}