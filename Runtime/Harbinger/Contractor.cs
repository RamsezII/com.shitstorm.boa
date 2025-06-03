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
        public readonly IEnumerator<Contract.Status> routine;

        public string error;

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            _id = 0;
        }

        //----------------------------------------------------------------------------------------------------------

        public Contractor(in Contract contract, in BoaReader reader, in Action<object> stdout)
        {
            id = _id.LoopID();

            this.contract = contract;
            this.reader = reader;
            this.stdout = stdout;

            if (contract.args != null)
                contract.args(this);

            if (contract.routine != null)
                routine = contract.routine(this);
        }

        //----------------------------------------------------------------------------------------------------------

        internal override IEnumerator<Contract.Status> EExecute()
        {
            contract.action?.Invoke(this);
            if (routine != null)
                while (routine.MoveNext())
                    yield return routine.Current;
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