using System.Collections.Generic;

namespace _BOA_
{
    public class LiteralExecutor : ExpressionExecutor
    {
        readonly object literal;

        //----------------------------------------------------------------------------------------------------------

        public LiteralExecutor(in Harbinger harbinger, in ScopeNode scope, in object literal) : base(harbinger, scope)
        {
            this.literal = literal;
        }

        //----------------------------------------------------------------------------------------------------------

        internal override bool IsMarkedAsOutput()
        {
            return false;
        }

        internal override IEnumerator<Contract.Status> EExecute()
        {
            yield return new Contract.Status(Contract.Status.States.ACTION_skip, output: literal);
        }
    }
}