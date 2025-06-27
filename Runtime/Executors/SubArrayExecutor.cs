using System;
using System.Collections.Generic;

namespace _BOA_
{
    internal sealed class SubArrayExecutor : ExpressionExecutor
    {
        readonly ExpressionExecutor expr_list;
        readonly ExpressionExecutor expr_access;
        public override Type OutputType() => expr_access?.OutputType();

        //----------------------------------------------------------------------------------------------------------

        public SubArrayExecutor(in Harbinger harbinger, in ScopeNode scope, in ExpressionExecutor array, in ExpressionExecutor index) : base(harbinger, scope)
        {
            expr_list = array;
            expr_access = index;
        }

        //----------------------------------------------------------------------------------------------------------

        public override IEnumerator<Contract.Status> EExecute()
        {
            using var routine_list = expr_list.EExecute();
            while (routine_list.MoveNext())
                yield return routine_list.Current;

            List<object> list = (List<object>)routine_list.Current.output;

            using var routine_access = expr_access.EExecute();
            while (routine_access.MoveNext())
                yield return routine_access.Current;

            int index = (int)routine_access.Current.output;
            yield return new Contract.Status(Contract.Status.States.ACTION_skip, output: list[index]);
        }
    }
}