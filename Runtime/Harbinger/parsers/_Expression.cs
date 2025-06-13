namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseExpression(BoaReader reader, in bool as_function_argument, out ExpressionExecutor expression, out string error)
        {
            if (TryParseAssignation(reader, out expression, out error) || error == null && TryParseOr(reader, out expression, out error))
            {
                if (as_function_argument && reader.strict_syntax && !reader.TryReadMatch(',') && !reader.TryPeekSpecific(')'))
                {
                    error ??= $"expected ',' or ')' after expression";
                    if (expression is ContractExecutor cont)
                        error += $" ('{cont.contract.name}')";
                    else
                        error += $" ('{expression.GetType()}')";
                }
                else
                {
                    if (reader.TryReadMatch('|'))
                        if (!reader.TryReadArgument(out string pipe_cont_name, out error, as_function_argument: false))
                            error ??= $"expected command after pipe operator '|'";
                        else if (!global_contracts.TryGetValue(pipe_cont_name, out var pipe_cont))
                            error ??= $"can not find command with name '{pipe_cont_name}'";
                        else
                        {
                            expression.pipe_next = new ContractExecutor(this, pipe_cont, reader, pipe_previous: expression, parse_arguments: true);
                            if (expression.pipe_next.error != null)
                            {
                                error ??= expression.pipe_next.error;
                                return false;
                            }
                            expression = new PipeExecutor(this, expression, expression.pipe_next);
                        }
                    return true;
                }
            }
            return false;
        }
    }
}