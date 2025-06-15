using System.Collections.Generic;

namespace _BOA_
{
    public class IncrementExecutor : ExpressionExecutor
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

        public IncrementExecutor(in Harbinger harbinger, in Executor caller, in BoaVariable variable, in Operators code) : base(harbinger, caller)
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