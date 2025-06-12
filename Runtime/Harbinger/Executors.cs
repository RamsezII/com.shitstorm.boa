using System;
using System.Collections.Generic;
using UnityEngine;

namespace _BOA_
{
    public abstract class Executor : IDisposable
    {
        public readonly Harbinger harbinger;
        public bool disposed;

        //----------------------------------------------------------------------------------------------------------

        public Executor(in Harbinger harbinger)
        {
            this.harbinger = harbinger;
        }

        //----------------------------------------------------------------------------------------------------------

        internal abstract IEnumerator<Contract.Status> EExecute(Action<object> after_execution = null);

        public static IEnumerator<Contract.Status> EExecute(Func<object, object> modify_output = null, params IEnumerator<Contract.Status>[] stack)
        {
            object data = null;

            if (stack != null && stack.Length > 0)
                for (int i = 0; i < stack.Length; i++)
                    if (stack[i] != null)
                    {
                        using var routine = stack[i];
                        while (routine.MoveNext())
                        {
                            data = routine.Current.data;
                            yield return routine.Current;
                        }
                    }

            if (modify_output != null)
                yield return new Contract.Status()
                {
                    state = Contract.Status.States.ACTION_skip,
                    data = modify_output?.Invoke(data),
                };
        }

        //----------------------------------------------------------------------------------------------------------

        public void Dispose()
        {
            if (disposed)
                return;
            disposed = true;
            OnDispose();
        }

        protected virtual void OnDispose()
        {
        }
    }

    public sealed class ContractExecutor : Executor
    {
        static ushort _id;
        public readonly ushort id;

        public readonly Contract contract;
        public readonly BoaReader reader;
        public readonly List<object> args = new();
        public string error;

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            _id = 0;
        }

        //----------------------------------------------------------------------------------------------------------

        public ContractExecutor(in Harbinger harbinger, in Contract contract, in BoaReader reader, in bool parse_arguments = true) : base(harbinger)
        {
            id = _id.LoopID();

            this.contract = contract;
            this.reader = reader;

            if (parse_arguments && contract != null)
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

    public sealed class BlockExecutor : Executor
    {
        internal readonly List<Executor> stack = new();

        //----------------------------------------------------------------------------------------------------------

        internal BlockExecutor(in Harbinger harbinger) : base(harbinger)
        {
        }

        //----------------------------------------------------------------------------------------------------------

        internal override IEnumerator<Contract.Status> EExecute(Action<object> end_action = null)
        {
            object data = null;
            for (int i = 0; i < stack.Count; i++)
            {
                var exe = stack[i];
                using var routine = exe.EExecute();
                while (routine.MoveNext())
                {
                    data = routine.Current.data;
                    yield return routine.Current;
                }
            }
            end_action?.Invoke(data);
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void OnDispose()
        {
            base.OnDispose();
            for (int i = 0; i < stack.Count; i++)
                stack[i].Dispose();
            stack.Clear();
        }
    }
}