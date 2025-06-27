using System;
using System.Collections.Generic;

namespace _BOA_
{
    public class ContractExecutor : ExpressionExecutor
    {
        public readonly Contract contract;
        public readonly BoaReader reader;
        public Executor arg_0;
        public readonly Dictionary<string, object> opts = new(StringComparer.Ordinal);
        public readonly List<object> args = new();
        public readonly ExpressionExecutor pipe_previous;
        public override string ToLog => $"'{base.ToLog}[{contract?.name}]'";
        public override Type OutputType() => contract?.get_output_type?.Invoke(this) ?? contract?.output_type;

        //----------------------------------------------------------------------------------------------------------

        public ContractExecutor(in Harbinger harbinger, in ScopeNode scope, in Contract contract, in BoaReader reader, in ExpressionExecutor pipe_previous = null, in bool parse_arguments = true) : base(harbinger, scope)
        {
            this.contract = contract;
            this.reader = reader;
            this.pipe_previous = pipe_previous;

            if (parse_arguments)
            {
                contract.opts?.Invoke(this);

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
                        harbinger.Stderr($"'{contract.name}' expected opening parenthesis '('");
                        return;
                    }

                    contract.args?.Invoke(this);

                    if (reader.sig_error != null)
                        return;

                    if ((expects_parenthesis || found_parenthesis) && !reader.TryReadChar_match(')', lint: reader.CloseBraquetLint()))
                    {
                        harbinger.Stderr($"'{contract.name}' expected closing parenthesis ')'");
                        return;
                    }
                }
            }
        }

        //----------------------------------------------------------------------------------------------------------

        internal override void MarkAsInstructionOutput()
        {
            if (contract.outputs_if_end_of_instruction)
                base.MarkAsInstructionOutput();
        }

        public override IEnumerator<Contract.Status> EExecute()
        {
            if (contract.routine != null)
            {
                using var routine = contract.routine(this);
                if (routine != null)
                    while (routine.MoveNext())
                        yield return routine.Current;
            }

            contract.action?.Invoke(this);

            if (contract.function != null)
                yield return new Contract.Status(Contract.Status.States.ACTION_skip, output: contract.function(this));
        }
    }
}