using System;
using System.Collections.Generic;
using UnityEngine;

namespace _BOA_
{
    public abstract class AbstractContractor : IDisposable
    {
        public bool disposed;
        public object result;
        internal abstract IEnumerator<Contract.Status> EExecute();

        //----------------------------------------------------------------------------------------------------------
        public IEnumerator<Contract.Status> ERoutinize(Action action)
        {
            var routine = EExecute();
            while (routine.MoveNext())
                yield return routine.Current;
            action?.Invoke();
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

    public sealed class Contractor : AbstractContractor
    {
        static ushort _id;
        public readonly ushort id;

        public readonly Contract contract;
        public readonly BoaReader reader;
        public readonly Action<object> stdout;
        public readonly List<object> args = new();

        public string error;

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            _id = 0;
        }

        //----------------------------------------------------------------------------------------------------------

        public Contractor(in Contract contract, in BoaReader reader, in Action<object> stdout, in bool parse_arguments = true)
        {
            id = _id.LoopID();

            this.contract = contract;
            this.reader = reader;
            this.stdout = stdout;

            if (parse_arguments)
                contract.args?.Invoke(this);
        }

        //----------------------------------------------------------------------------------------------------------

        internal override IEnumerator<Contract.Status> EExecute()
        {
            contract.action?.Invoke(this);
            if (contract.routine != null)
            {
                var routine = contract.routine(this);
                while (routine.MoveNext())
                    yield return routine.Current;
            }
        }
    }

    internal sealed class Contractor_value<T> : AbstractContractor
    {
        internal readonly Literal<T> literal;

        //----------------------------------------------------------------------------------------------------------

        public Contractor_value(in Literal<T> literal)
        {
            this.literal = literal;
        }

        //----------------------------------------------------------------------------------------------------------

        internal override IEnumerator<Contract.Status> EExecute()
        {
            result = literal.Value;
            yield break;
        }
    }

    public sealed class BodyContractor : AbstractContractor
    {
        internal readonly List<AbstractContractor> stack = new();

        //----------------------------------------------------------------------------------------------------------

        internal override IEnumerator<Contract.Status> EExecute()
        {
            for (int i = 0; i < stack.Count; i++)
            {
                var contractor = stack[i];
                var execution = contractor.EExecute();
                while (!contractor.disposed && execution.MoveNext())
                    yield return execution.Current;
            }
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