using System;
using System.Collections.Generic;
using UnityEngine;

namespace _BOA_
{
    public class Contract
    {
        [Serializable]
        public struct Status : IEquatable<Status>
        {
            public enum States
            {
                BLOCKING,
                ACTION_skip,
                WAIT_FOR_STDIN,
                BACKGROUND,
                DAEMON,
            }

            static byte _id;
            public readonly byte id;
            public readonly States state;
            public string prefixe_text, prefixe_lint;
            [Range(0, 1)] public float progress;
            public object output;

            //----------------------------------------------------------------------------------------------------------

            public Status(in States state, in string prefixe_text = default, in string prefixe_lint = default, in float progress = default, in object output = default)
            {
                id = _id++;
                this.state = state;
                this.prefixe_text = prefixe_text ?? string.Empty;
                this.prefixe_lint = prefixe_lint ?? prefixe_text;
                this.progress = progress;
                this.output = output;
            }

            //----------------------------------------------------------------------------------------------------------

            public readonly bool Equals(Status other) => id == other.id && state == other.state && progress == other.progress;
        }

        public readonly string name;
        internal readonly int min_args, max_args;
        public readonly bool function_style_arguments, no_semicolon_required, no_parenthesis, outputs_if_end_of_instruction;
        public readonly Action<ContractExecutor> opts, args;
        internal readonly Action<ContractExecutor> action;
        internal readonly Func<ContractExecutor, object> function;
        internal readonly Func<ContractExecutor, IEnumerator<Status>> routine;

        //----------------------------------------------------------------------------------------------------------

        public Contract(in string name,
            in int min_args = 0,
            in int max_args = 0,
            in bool function_style_arguments = true,
            in bool no_semicolon_required = false,
            in bool no_parenthesis = false,
            in bool outputs_if_end_of_instruction = false,
            in Action<ContractExecutor> opts = null,
            in Action<ContractExecutor> args = null,
            in Action<ContractExecutor> action = null,
            in Func<ContractExecutor, object> function = null,
            in Func<ContractExecutor, IEnumerator<Status>> routine = null
            )
        {
            this.name = name;
            this.min_args = Mathf.Min(min_args, max_args);
            this.max_args = Mathf.Max(min_args, max_args);
            this.function_style_arguments = function_style_arguments;
            this.no_semicolon_required = no_semicolon_required;
            this.no_parenthesis = no_parenthesis;
            this.outputs_if_end_of_instruction = outputs_if_end_of_instruction;
            this.opts = opts;
            this.args = args;
            this.action = action;
            this.function = function;
            this.routine = routine;
        }
    }
}