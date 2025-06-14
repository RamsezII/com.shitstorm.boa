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

        internal override IEnumerator<Contract.Status> EExecute()
        {
            for (int i = 0; i < stack.Count; i++)
            {
                var exe = stack[i];
                var routine = exe.EExecute();
                while (routine.MoveNext())
                    yield return routine.Current;
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