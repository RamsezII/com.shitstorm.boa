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
            if (!harbinger.TryParseExpression(reader, scope, false, null, out executor, type_check: false))
                reader.Stderr($"expected expression.");
        }

        //----------------------------------------------------------------------------------------------------------

        public override IEnumerator<Contract.Status> EExecute() => executor.EExecute();
    }
}