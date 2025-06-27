using System;
using System.Collections.Generic;

namespace _BOA_
{
    internal class PipeExecutor : ExpressionExecutor
    {
        readonly Executor previous;
        readonly ContractExecutor next;
        public override Type OutputType() => next?.OutputType();

        //----------------------------------------------------------------------------------------------------------

        public PipeExecutor(in Harbinger harbinger, in ScopeNode scope, in Executor previous, in ContractExecutor next) : base(harbinger, scope)
        {
            this.previous = previous;
            this.next = next;
        }

        //----------------------------------------------------------------------------------------------------------

        public override IEnumerator<Contract.Status> EExecute()
        {
            using var routine1 = previous.EExecute();
            while (routine1.MoveNext())
                yield return routine1.Current;

            next.arg_0 = new LiteralExecutor(harbinger, scope, routine1.Current.output);

            using var routine2 = next.EExecute();
            while (routine2.MoveNext())
                yield return routine2.Current;
        }
    }
}