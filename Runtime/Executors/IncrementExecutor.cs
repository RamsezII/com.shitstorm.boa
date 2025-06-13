using System.Collections.Generic;

namespace _BOA_
{
    public class IncrementExecutor : ExpressionExecutor
    {
        public enum Codes : byte
        {
            None,
            AddBefore,
            SubBefore,
            AddAfter,
            SubAfter,
        }

        readonly Codes code;
        readonly BoaVar variable;

        //----------------------------------------------------------------------------------------------------------

        public IncrementExecutor(in Harbinger harbinger, in BoaVar variable, in Codes code) : base(harbinger)
        {
            this.code = code;
            this.variable = variable;
        }

        //----------------------------------------------------------------------------------------------------------

        internal override IEnumerator<Contract.Status> EExecute()
        {
            switch (code)
            {
                case Codes.AddBefore:
                    variable.value = 1 + (int)variable.value;
                    break;
                case Codes.SubBefore:
                    variable.value = 1 - (int)variable.value;
                    break;
            }

            yield return new Contract.Status(Contract.Status.States.ACTION_skip, data: variable.value);

            switch (code)
            {
                case Codes.AddAfter:
                    variable.value = 1 + (int)variable.value;
                    break;
                case Codes.SubAfter:
                    variable.value = 1 - (int)variable.value;
                    break;
            }
        }
    }
}