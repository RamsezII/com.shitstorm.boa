using System.Collections.Generic;

namespace _BOA_
{
    public sealed class BlockExecutor : Executor
    {
        internal readonly List<Executor> stack = new();

        //----------------------------------------------------------------------------------------------------------

        internal BlockExecutor(in Harbinger harbinger, in ScopeNode scope) : base(harbinger, scope)
        {
        }

        //----------------------------------------------------------------------------------------------------------

        internal override void MarkAsInstructionOutput()
        {
            base.MarkAsInstructionOutput();
            if (stack.Count > 0)
                stack[^1].MarkAsInstructionOutput();
        }

        internal override bool IsMarkedAsOutput()
        {
            if (base.IsMarkedAsOutput())
                if (stack.Count > 0)
                    return stack[^1].IsMarkedAsOutput();
            return false;
        }

        public override IEnumerator<Contract.Status> EExecute()
        {
            for (int i = 0; i < stack.Count; i++)
            {
                var exe = stack[i];

                using var routine = exe.EExecute();
                while (routine.MoveNext())
                    yield return routine.Current;

                if (exe.IsMarkedAsOutput())
                    exe.harbinger.shell.AddLine(routine.Current.output);
            }
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void OnDispose()
        {
            base.OnDispose();
            for (int i = 0; i < stack.Count; i++)
                stack[i].Dispose();
            stack.Clear();
        }
    }
}