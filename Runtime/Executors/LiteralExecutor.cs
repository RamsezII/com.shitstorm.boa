using System.Collections.Generic;

namespace _BOA_
{
    public class LiteralExecutor : ExpressionExecutor
    {
        readonly object literal;

        //----------------------------------------------------------------------------------------------------------

        public LiteralExecutor(in Harbinger harbinger, in Executor parent, in object literal) : base(harbinger, parent)
        {
            this.literal = literal;
        }

        //----------------------------------------------------------------------------------------------------------

        internal override IEnumerator<Contract.Status> EExecute()
        {
            yield return new Contract.Status(Contract.Status.States.ACTION_skip, data: literal);
        }
    }
}