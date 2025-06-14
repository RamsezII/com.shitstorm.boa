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
                ACTION_skip,
                WAIT_FOR_STDIN,
                BACKGROUND,
                DAEMON,
            }

            public readonly States state;
            public string prefixe;
            [Range(0, 1)] public float progress;
            public object data;

            //----------------------------------------------------------------------------------------------------------

            public Status(in States state, in string prefixe = default, in float progress = default, in object data = default)
            {
                this.state = state;
                this.prefixe = prefixe;
                this.progress = progress;
                this.data = data;
            }
        }

        public readonly string name;
        internal readonly int min_args, max_args;
        public readonly Action<ContractExecutor> args;
        public readonly bool function_style_arguments, no_semicolon_required, no_parenthesis;
        internal readonly Func<ContractExecutor, IEnumerator<Status>> routine;

        //----------------------------------------------------------------------------------------------------------

        public Contract(in string name,
            in int min_args = 0,
            in int max_args = 0,
            in bool function_style_arguments = true,
            in bool no_semicolon_required = false,
            in bool no_parenthesis = false,
            in Action<ContractExecutor> args = null,
            in Func<ContractExecutor, IEnumerator<Status>> routine = null
            )
        {
            this.name = name;
            this.min_args = Mathf.Min(min_args, max_args);
            this.max_args = Mathf.Max(min_args, max_args);
            this.function_style_arguments = function_style_arguments;
            this.no_semicolon_required = no_semicolon_required;
            this.no_parenthesis = no_parenthesis;
            this.args = args;
            this.routine = routine;
        }
    }
}