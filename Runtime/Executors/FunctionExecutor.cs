using System;
using System.Collections.Generic;

namespace _BOA_
{
    internal sealed class FunctionExecutor : ExpressionExecutor
    {
        public FunctionExecutor(in Harbinger harbinger, in Executor caller) : base(harbinger, caller)
        {
        }

        //----------------------------------------------------------------------------------------------------------

        internal override IEnumerator<Contract.Status> EExecute() => throw new NotImplementedException();
    }
}