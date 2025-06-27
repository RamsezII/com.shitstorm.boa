using System;
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
        public override Type OutputType() => expr?.OutputType();

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
                    Operators.Sub => data switch { int i => -i, float f => -f, _ => data, },
                    Operators.Not => data switch { bool b => !b, _ => !data.ToBool(), },
                    _ => data,
                },
            };
        }
    }
}