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

        internal abstract IEnumerator<Contract.Status> EExecute(Action<object> on_done = null);

        public static IEnumerator<Contract.Status> EExecute(params IEnumerator<Contract.Status>[] stack)
        {
            if (stack != null && stack.Length > 0)
                for (int i = 0; i < stack.Length; i++)
                    if (stack[i] != null)
                    {
                        var routine = stack[i];
                        while (routine.MoveNext())
                            yield return routine.Current;
                    }
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

            if (parse_arguments)
                contract?.args?.Invoke(this);
        }

        //----------------------------------------------------------------------------------------------------------

        internal override IEnumerator<Contract.Status> EExecute(Action<object> on_done)
        {
            if (contract != null)
                if (contract.action != null)
                {
                    object data = contract.action(this);
                    yield return new Contract.Status() { data = data, };
                    on_done?.Invoke(data);
                }
                else if (contract.routine != null)
                {
                    var routine = contract.routine(this);
                    while (routine.MoveNext())
                        yield return routine.Current;
                    on_done?.Invoke(routine.Current.data);
                }
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

        internal override IEnumerator<Contract.Status> EExecute(Action<object> on_done = null)
        {
            IEnumerator<Contract.Status> routine = null;
            for (int i = 0; i < stack.Count; i++)
            {
                var exe = stack[i];
                routine = exe.EExecute();
                while (!exe.disposed && routine.MoveNext())
                    yield return routine.Current;
            }
            on_done?.Invoke(routine?.Current.data);
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