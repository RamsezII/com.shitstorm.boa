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
        readonly BoaVariable variable;

        //----------------------------------------------------------------------------------------------------------

        internal IncrementExecutor(in Harbinger harbinger, in ScopeNode scope, in BoaVariable variable, in Operators code) : base(harbinger, scope)
        {
            this.code = code;
            this.variable = variable;
        }

        //----------------------------------------------------------------------------------------------------------

        internal override IEnumerator<Contract.Status> EExecute()
        {
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