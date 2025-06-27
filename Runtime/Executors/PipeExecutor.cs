using System;
using System.Collections.Generic;

namespace _BOA_
{
    internal class PipeExecutor : ExpressionExecutor
    {
        readonly ExpressionExecutor left_expr;
        public override Type OutputType() => left_expr?.pipe_next?.OutputType();

        //----------------------------------------------------------------------------------------------------------

        public PipeExecutor(in Harbinger harbinger, in ScopeNode scope, in ExpressionExecutor left_expr) : base(harbinger, scope)
        {
            this.left_expr = left_expr;
        }

        //----------------------------------------------------------------------------------------------------------

        public override IEnumerator<Contract.Status> EExecute()
        {
            using var routine1 = left_expr.EExecute();
            while (routine1.MoveNext())
                yield return routine1.Current;

            left_expr.pipe_next.arg_0 = new LiteralExecutor(harbinger, scope, routine1.Current.output);

            using var routine2 = left_expr.pipe_next.EExecute();
            while (routine2.MoveNext())
                yield return routine2.Current;
        }
    }
}