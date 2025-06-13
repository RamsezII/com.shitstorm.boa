using System;
using System.Collections.Generic;

namespace _BOA_
{
    public sealed class BlockExecutor : Executor
    {
        internal readonly List<Executor> stack = new();

        //----------------------------------------------------------------------------------------------------------

        internal BlockExecutor(in Harbinger harbinger) : base(harbinger)
        {
        }

        //----------------------------------------------------------------------------------------------------------

        internal override IEnumerator<Contract.Status> EExecute(Action<object> end_action = null)
        {
            object data = null;
            for (int i = 0; i < stack.Count; i++)
            {
                var exe = stack[i];
                using var routine = exe.EExecute();
                while (routine.MoveNext())
                {
                    data = routine.Current.data;
                    yield return routine.Current;
                }
            }
            end_action?.Invoke(data);
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