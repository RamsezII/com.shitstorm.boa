using System.Collections.Generic;

namespace _BOA_
{
    public class VariableExecutor : ExpressionExecutor
    {
        internal readonly BoaVar variable;

        //----------------------------------------------------------------------------------------------------------

        public VariableExecutor(in Harbinger harbinger, in BoaVar variable) : base(harbinger)
        {
            this.variable = variable;
        }

        //----------------------------------------------------------------------------------------------------------

        internal override IEnumerator<Contract.Status> EExecute()
        {
            yield return new Contract.Status(Contract.Status.States.ACTION_skip, data: variable.value);
        }
    }
}