using System.Collections.Generic;

namespace _BOA_
{
    internal class UnaryExecutor : ExpressionExecutor
    {
        public enum Operators : byte
        {
            Add,
            Sub,
            Not,
        }

        readonly Operators code;
        readonly ExpressionExecutor expr;

        //----------------------------------------------------------------------------------------------------------

        public UnaryExecutor(in Harbinger harbinger, in ScopeNode scope, in ExpressionExecutor expr, in Operators code) : base(harbinger, scope)
        {
            this.code = code;
            this.expr = expr;
        }

        //----------------------------------------------------------------------------------------------------------

        public override IEnumerator<Contract.Status> EExecute()
        {
            using var routine = expr.EExecute();
            while (routine.MoveNext())
                yield return routine.Current;

            object data = routine.Current.output;

            yield return new Contract.Status(Contract.Status.States.ACTION_skip)
            {
                output = code switch
                {
                    Operators.Add => +(int)data,
                    Operators.Sub => -(int)data,
                    Operators.Not => !(bool)data,
                    _ => data,
                },
            };
        }
    }
}