using System.Collections.Generic;

namespace _BOA_
{
    public class VariableExecutor : ExpressionExecutor
    {
        internal readonly BoaVariable variable;

        //----------------------------------------------------------------------------------------------------------

        public VariableExecutor(in Harbinger harbinger, in Executor caller, in BoaVariable variable) : base(harbinger, caller)
        {
            this.variable = variable;
        }

        //----------------------------------------------------------------------------------------------------------

        internal override IEnumerator<Contract.Status> EExecute()
        {
            yield return new Contract.Status(Contract.Status.States.ACTION_skip, output: variable.value);
        }
    }
}