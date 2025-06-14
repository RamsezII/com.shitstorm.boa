namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseExpression(in BoaReader reader, in Executor parent, in bool as_function_argument, out ExpressionExecutor expression, out string error)
        {
            if (TryParseAssignation(reader, parent, out expression, out error) || error == null && TryParseOr(reader, parent, out expression, out error))
            {
                if (as_function_argument && reader.strict_syntax && !reader.TryReadChar_match(',') && !reader.TryPeekChar_match(')'))
                {
                    error ??= $"expected ',' or ')' after expression";
                    if (expression is ContractExecutor cont)
                        error += $" ('{cont.contract.name}')";
                    else
                        error += $" ('{expression.GetType()}')";
                }
                else if (TryPipe(reader, parent, ref expression, out error))
                    return true;
            }
            return false;
        }

        bool TryPipe(in BoaReader reader, in Executor parent, ref ExpressionExecutor expression, out string error)
        {
            error = null;

            if (!reader.TryReadChar_match('|'))
                return true;
            else if (!reader.TryReadArgument(out string pipe_cont_name, out error, as_function_argument: false))
                error ??= $"expected command after pipe operator '|'";
            else if (!global_contracts.TryGetValue(pipe_cont_name, out var pipe_cont))
                error ??= $"can not find command with name '{pipe_cont_name}'";
            else
            {
                expression.pipe_next = new ContractExecutor(this, parent, pipe_cont, reader, pipe_previous: expression);
                if (expression.pipe_next.error != null)
                {
                    error ??= expression.pipe_next.error;
                    return false;
                }

                expression = new PipeExecutor(this, parent, expression, expression.pipe_next);

                if (!TryPipe(reader, parent, ref expression, out error) || error != null)
                {
                    error ??= $"could not parse pipe statement";
                    return false;
                }

                return true;
            }

            return false;
        }
    }
}