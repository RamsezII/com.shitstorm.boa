using System;
using System.Collections.Generic;
using UnityEngine;

namespace _BOA_
{
    public sealed class ContractExecutor : Executor
    {
        static ushort _id;
        public readonly ushort id;

        public readonly Contract contract;
        public readonly BoaReader reader;
        public readonly List<object> args = new();
        public readonly ContractExecutor pipe_previous, pipe_next;
        public string error;

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            _id = 0;
        }

        //----------------------------------------------------------------------------------------------------------

        public ContractExecutor(in Harbinger harbinger, in Contract contract, in BoaReader reader, in ContractExecutor pipe_previous = null, in bool parse_arguments = true) : base(harbinger)
        {
            id = _id.LoopID();

            this.contract = contract;
            this.reader = reader;
            this.pipe_previous = pipe_previous;

            if (parse_arguments)
            {
                if (contract != null)
                {
                    bool expects_parenthesis = reader.strict_syntax && contract.function_style_arguments;

                    if (expects_parenthesis && !reader.TryReadMatch('('))
                    {
                        error = $"'{contract.name}' expected opening parenthesis '('";
                        return;
                    }

                    contract?.args?.Invoke(this);

                    if (error != null)
                        return;

                    if (expects_parenthesis && !reader.TryReadMatch(')'))
                    {
                        error = $"'{contract.name}' expected closing parenthesis ')'";
                        return;
                    }
                }
            }
        }

        //----------------------------------------------------------------------------------------------------------

        internal override IEnumerator<Contract.Status> EExecute(Action<object> end_action = null)
        {
            object data = null;

            if (contract != null)
            {
                if (contract.action != null)
                {
                    data = contract.action(this);
                    yield return new Contract.Status()
                    {
                        state = Contract.Status.States.ACTION_skip,
                        data = data,
                    };
                }

                if (contract.routine != null)
                {
                    using var routine = contract.routine(this);
                    while (routine.MoveNext())
                    {
                        data = routine.Current.data;
                        yield return routine.Current;
                    }
                }
            }

            end_action?.Invoke(data);
        }
    }
}