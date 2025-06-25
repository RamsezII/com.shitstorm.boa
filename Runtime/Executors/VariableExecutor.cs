using System.Collections.Generic;

namespace _BOA_
{
    internal class VariableExecutor : ExpressionExecutor
    {
        internal readonly string var_name;

        //----------------------------------------------------------------------------------------------------------

        public VariableExecutor(in Harbinger harbinger, in ScopeNode scope, in string var_name) : base(harbinger, scope)
        {
            this.var_name = var_name;
        }

        //----------------------------------------------------------------------------------------------------------

        internal override bool IsMarkedAsOutput()
        {
            return false;
        }

        internal override IEnumerator<Contract.Status> EExecute()
        {
            if (scope.TryGetVariable(var_name, out var variable))
                yield return new Contract.Status(Contract.Status.States.ACTION_skip, output: variable.value);
            else
                harbinger.Stderr($"couldnt not find variable '{var_name}'");
        }
    }
}