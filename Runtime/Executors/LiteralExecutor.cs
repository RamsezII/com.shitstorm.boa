using System;
using System.Collections.Generic;

namespace _BOA_
{
    public class LiteralExecutor : ExpressionExecutor
    {
        readonly object value;
        public override Type OutputType() => value?.GetType() ?? typeof(object);

        //----------------------------------------------------------------------------------------------------------

        public LiteralExecutor(in Harbinger harbinger, in ScopeNode scope, in object value) : base(harbinger, scope)
        {
            this.value = value;
        }

        //----------------------------------------------------------------------------------------------------------

        internal override bool IsMarkedAsOutput()
        {
            return false;
        }

        public override IEnumerator<Contract.Status> EExecute()
        {
            yield return new Contract.Status(Contract.Status.States.ACTION_skip, output: value);
        }
    }
}