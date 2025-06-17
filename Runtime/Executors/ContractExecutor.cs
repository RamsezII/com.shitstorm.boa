using System.Collections.Generic;

namespace _BOA_
{
    public class ContractExecutor : ExpressionExecutor
    {
        public readonly Contract contract;
        public readonly BoaReader reader;
        public Executor arg_0;
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
                if (contract.no_parenthesis)
                    contract.args?.Invoke(this);
                else
                {
                    bool expects_parenthesis = reader.strict_syntax && contract.function_style_arguments;
                    bool found_parenthesis = reader.TryReadChar_match('(');

                    if (found_parenthesis)
                        reader.LintOpeningBraquet();

                    if (expects_parenthesis && !found_parenthesis)
                    {
                        error ??= $"'{contract.name}' expected opening parenthesis '('";
                        return;
                    }

                    contract.args?.Invoke(this);

                    if (error != null)
                        return;

                    if ((expects_parenthesis || found_parenthesis) && !reader.TryReadChar_match(')', lint: reader.CloseBraquetLint()))
                    {
                        error ??= $"'{contract.name}' expected closing parenthesis ')'";
                        return;
                    }
                }
        }

        //----------------------------------------------------------------------------------------------------------

        internal override IEnumerator<Contract.Status> EExecute()
        {
            if (contract.action != null)
                yield return new Contract.Status(Contract.Status.States.ACTION_skip, output: contract.action(this));

            if (contract.routine != null)
            {
                using var routine = contract.routine(this);
                if (routine != null)
                    while (routine.MoveNext())
                        yield return routine.Current;
            }
        }
    }
}