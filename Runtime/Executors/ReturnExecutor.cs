using System;
using System.Collections.Generic;

namespace _BOA_
{
    sealed internal class ReturnExecutor : ExpressionExecutor
    {
        readonly ExpressionExecutor executor;
        public override Type OutputType() => executor?.OutputType();

        //----------------------------------------------------------------------------------------------------------

        public ReturnExecutor(in Harbinger harbinger, in ScopeNode scope, in BoaReader reader) : base(harbinger, scope)
        {
            if (!harbinger.TryParseExpression(reader, scope, false, out executor))
                reader.Stderr($"expected expression.");
        }

        //----------------------------------------------------------------------------------------------------------

        public override IEnumerator<Contract.Status> EExecute() => executor.EExecute();
    }
}