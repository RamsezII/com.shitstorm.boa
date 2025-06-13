using System;
using System.Collections.Generic;

namespace _BOA_
{
    public class PipeExecutor : Executor
    {
        readonly Executor previous;
        readonly ContractExecutor next;

        //----------------------------------------------------------------------------------------------------------

        public PipeExecutor(in Harbinger harbinger, in Executor previous, in ContractExecutor next) : base(harbinger)
        {
            this.previous = previous;
            this.next = next;
        }

        //----------------------------------------------------------------------------------------------------------

        internal override IEnumerator<Contract.Status> EExecute(Action<object> after_execution = null)
        {
            IEnumerator<Contract.Status> routine = null;
            try
            {
                routine = previous.EExecute();
                while (routine.MoveNext())
                    yield return routine.Current;

                if (next.args.Count > 0)
                    next.args[0] = routine.Current.data;

                routine = next.EExecute();
                while (routine.MoveNext())
                    yield return routine.Current;
            }
            finally
            {
                routine.Dispose();
            }
        }
    }
}