using System.Collections.Generic;

namespace _BOA_
{
    internal sealed class TernaryOpExecutor : ExpressionExecutor
    {
        readonly ExpressionExecutor cond, _if, _else;

        //----------------------------------------------------------------------------------------------------------

        public TernaryOpExecutor(in Harbinger harbinger, in ScopeNode scope, in ExpressionExecutor cond, in ExpressionExecutor _if, in ExpressionExecutor _else) : base(harbinger, scope)
        {
            this.cond = cond;
            this._if = _if;
            this._else = _else;
        }

        //----------------------------------------------------------------------------------------------------------

        public override IEnumerator<Contract.Status> EExecute()
        {
            using var routine1 = cond.EExecute();
            while (routine1.MoveNext())
                yield return routine1.Current;

            bool _bool = routine1.Current.output.ToBool();

            using var routine2 = (_bool ? _if : _else).EExecute();
            while (routine2.MoveNext())
                yield return routine2.Current;
        }
    }
}