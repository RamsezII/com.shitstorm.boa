using System.Collections.Generic;

namespace _BOA_
{
    public class ContractExecutor : ExpressionExecutor
    {
        public readonly Contract contract;
        public readonly BoaReader reader;
        public readonly List<object> args = new();
        public readonly ExpressionExecutor pipe_previous;
        public override string ToLog => $"'{base.ToLog}[{contract?.name}]'";

        //----------------------------------------------------------------------------------------------------------

        public ContractExecutor(in Harbinger harbinger, in ScopeNode scope, in Contract contract, in BoaReader reader, in ExpressionExecutor pipe_previous = null, in bool parse_arguments = true) : base(harbinger, scope)
        {
            this.contract = contract;
            this.reader = reader;
            this.pipe_previous = pipe_previous;

            if (parse_arguments)
                if (contract != null && contract.no_parenthesis)
                    contract?.args?.Invoke(this);
                else
                {
                    bool expects_parenthesis = reader.strict_syntax && contract.function_style_arguments;
                    bool found_parenthesis = reader.TryReadChar_match('(');

                    if (expects_parenthesis && !found_parenthesis)
                    {
                        error ??= $"'{contract.name}' expected opening parenthesis '('";
                        return;
                    }

                    contract?.args?.Invoke(this);

                    if (error != null)
                        return;

                    if ((expects_parenthesis || found_parenthesis) && !reader.TryReadChar_match(')'))
                    {
                        error ??= $"'{contract.name}' expected closing parenthesis ')'";
                        return;
                    }
                }
        }

        //----------------------------------------------------------------------------------------------------------

        internal override IEnumerator<Contract.Status> EExecute()
        {
            if (contract.routine != null)
            {
                using var routine = contract.routine(this);
                while (routine.MoveNext())
                    yield return routine.Current;
            }
        }
    }
}