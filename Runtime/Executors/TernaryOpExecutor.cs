using System.Collections.Generic;

namespace _BOA_
{
    internal sealed class TernaryOpExecutor : ExpressionExecutor
    {
        readonly ExpressionExecutor cond, _if, _else;

        //----------------------------------------------------------------------------------------------------------

        public TernaryOpExecutor(in Harbinger harbinger, in Executor caller, in ExpressionExecutor cond, in ExpressionExecutor _if, in ExpressionExecutor _else) : base(harbinger, caller)
        {
            this.cond = cond;
            this._if = _if;
            this._else = _else;
        }

        //----------------------------------------------------------------------------------------------------------

        internal override IEnumerator<Contract.Status> EExecute()
        {
            var routine = cond.EExecute();
            while (routine.MoveNext())
                yield return routine.Current;

            bool _bool = routine.Current.output.ToBool();

            routine = (_bool ? _if : _else).EExecute();
            while (routine.MoveNext())
                yield return routine.Current;
        }
    }
}