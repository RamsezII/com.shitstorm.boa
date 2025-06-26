using System.Collections.Generic;
using System.Text;

namespace _BOA_
{
    internal sealed class StringExecutor : ExpressionExecutor
    {
        readonly List<Executor> stack;

        //----------------------------------------------------------------------------------------------------------

        public StringExecutor(in Harbinger harbinger, in ScopeNode scope, in List<Executor> stack) : base(harbinger, scope)
        {
            this.stack = stack;
        }

        //----------------------------------------------------------------------------------------------------------

        internal override void MarkAsInstructionOutput()
        {
            base.MarkAsInstructionOutput();
            is_instruction_output = false;
        }

        internal override bool IsMarkedAsOutput()
        {
            return false;
        }

        public override IEnumerator<Contract.Status> EExecute()
        {
            StringBuilder sb = new();

            for (int i = 0; i < stack.Count; i++)
            {
                using var routine = stack[i].EExecute();

                while (routine.MoveNext())
                    yield return routine.Current;

                if (routine.Current.output != null)
                    sb.Append(routine.Current.output.ToString());
            }

            string output = sb.ToString();
            yield return new Contract.Status(Contract.Status.States.ACTION_skip, output: output);
        }
    }
}