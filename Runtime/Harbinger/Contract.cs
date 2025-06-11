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
        internal readonly int min_args, max_args;
        public readonly Action<ContractExecutor> args;
        public readonly bool expects_parenthesis;
        internal readonly Func<ContractExecutor, object> action;
        internal readonly Func<ContractExecutor, IEnumerator<Status>> routine;

        //----------------------------------------------------------------------------------------------------------

        public Contract(in string name,
            in int min_args = 0,
            in int max_args = 0,
            in bool expects_parenthesis = true,
            in Action<ContractExecutor> args = null,
            in Func<ContractExecutor, object> action = null,
            in Func<ContractExecutor, IEnumerator<Status>> routine = null
            )
        {
            this.name = name;
            this.min_args = Mathf.Min(min_args, max_args);
            this.max_args = Mathf.Max(min_args, max_args);
            this.expects_parenthesis = expects_parenthesis;
            this.args = args;
            this.action = action;
            this.routine = routine;
        }
    }
}