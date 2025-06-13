using System.Collections.Generic;

namespace _BOA_
{
    public class PipeExecutor : ExpressionExecutor
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

        internal override IEnumerator<Contract.Status> EExecute()
        {
            IEnumerator<Contract.Status> routine = null;
            try
            {
                routine = previous.EExecute();
                while (routine.MoveNext())
                    yield return routine.Current;

                if (next.args.Count == 0)
                    next.args.Add(null);
                next.args[0] = new LiteralExecutor(harbinger, routine.Current.data);

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