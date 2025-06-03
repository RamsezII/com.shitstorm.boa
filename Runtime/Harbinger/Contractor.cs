using System;
using System.Collections.Generic;
using UnityEngine;

namespace _BOA_
{
    public abstract class AbstractContractor
    {
        public object result;
        internal abstract IEnumerator<Contract.Status> EExecute();
    }

    public sealed class Contractor : AbstractContractor
    {
        static ushort _id;
        public readonly ushort id;

        public readonly Contract contract;
        public readonly Action<object> stdout;
        public readonly List<object> args;
        public readonly IEnumerator<Contract.Status> routine;

        public readonly BoaReader reader;

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
            {
                args = new();
                contract.args(this);
            }

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

    public sealed class MegaContractor : AbstractContractor
    {
        internal readonly List<AbstractContractor> stack = new();

        //----------------------------------------------------------------------------------------------------------

        internal override IEnumerator<Contract.Status> EExecute()
        {
            int stack_i = 0;
            while (stack_i < stack.Count)
            {
                var execution = stack[stack_i++].EExecute();
                while (execution.MoveNext())
                    yield return execution.Current;
            }
        }
    }
}