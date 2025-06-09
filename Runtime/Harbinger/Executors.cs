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

        public static IEnumerator<Contract.Status> EExecute(Func<object> on_all_done, params (Executor executor, Action<object> on_done)[] stack)
        {
            if (stack != null && stack.Length > 0)
                for (int i = 0; i < stack.Length; i++)
                    if (stack[i].executor != null)
                    {
                        var (executor, on_done) = stack[i];
                        var routine = executor.EExecute(on_done);
                        while (routine.MoveNext())
                            yield return routine.Current;
                    }
            yield return new() { data = on_all_done?.Invoke(), };
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

        internal override IEnumerator<Contract.Status> EExecute(Action<object> on_done = null)
        {
            object result = null;
            if (contract != null)
                if (contract.action != null)
                    result = contract.action(this);
                else if (contract.routine != null)
                {
                    var routine = contract.routine(this);
                    while (routine.MoveNext())
                    {
                        result = routine.Current.data;
                        yield return routine.Current;
                    }
                }
            on_done?.Invoke(result);
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
            object result = null;
            for (int i = 0; i < stack.Count; i++)
            {
                var exe = stack[i];
                var routine = exe.EExecute(null);
                while (!exe.disposed && routine.MoveNext())
                    yield return routine.Current;
                result = routine.Current.data;
            }
            on_done?.Invoke(result);
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