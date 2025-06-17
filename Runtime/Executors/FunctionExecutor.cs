using System;
using System.Collections.Generic;

namespace _BOA_
{
    internal sealed class FunctionExecutor : ExpressionExecutor
    {
        public FunctionExecutor(in Harbinger harbinger, in ScopeNode scope) : base(harbinger, scope)
        {
        }

        //----------------------------------------------------------------------------------------------------------

        internal override IEnumerator<Contract.Status> EExecute() => throw new NotImplementedException();
    }
}