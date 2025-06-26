using System.Collections.Generic;

namespace _BOA_
{
    internal class IncrementExecutor : ExpressionExecutor
    {
        public enum Operators : byte
        {
            None,
            AddBefore,
            SubBefore,
            AddAfter,
            SubAfter,
        }

        readonly Operators code;
        readonly string var_name;

        //----------------------------------------------------------------------------------------------------------

        internal IncrementExecutor(in Harbinger harbinger, in ScopeNode scope, in string var_name, in Operators code) : base(harbinger, scope)
        {
            this.code = code;
            this.var_name = var_name;
        }

        //----------------------------------------------------------------------------------------------------------

        public override IEnumerator<Contract.Status> EExecute()
        {
            if (!scope.TryGetVariable(var_name, out var variable))
            {
                harbinger.Stderr($"Could not find variable '{var_name}'.");
                yield break;
            }

            switch (code)
            {
                case Operators.AddBefore:
                    variable.value = 1 + (int)variable.value;
                    break;
                case Operators.SubBefore:
                    variable.value = 1 - (int)variable.value;
                    break;
            }

            yield return new Contract.Status(Contract.Status.States.ACTION_skip, output: variable.value);

            switch (code)
            {
                case Operators.AddAfter:
                    variable.value = 1 + (int)variable.value;
                    break;
                case Operators.SubAfter:
                    variable.value = 1 - (int)variable.value;
                    break;
            }
        }
    }
}