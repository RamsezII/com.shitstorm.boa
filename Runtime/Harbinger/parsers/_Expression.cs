namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseExpression(in BoaReader reader, in Executor caller, in bool as_function_argument, out ExpressionExecutor expression)
        {
            if (TryParseAssignation(reader, caller, out expression) || reader.error == null && TryParseOr(reader, caller, out expression))
            {
                if (as_function_argument && reader.strict_syntax)
                    if (!reader.TryReadChar_match(',') && !reader.TryPeekChar_match(')'))
                    {
                        reader.error ??= $"expected ',' or ')' after expression";
                        if (expression is ContractExecutor cont)
                            reader.error += $" ('{cont.contract.name}')";
                        else
                            reader.error += $" ('{expression.GetType()}')";
                        goto failure;
                    }

                if (TryPipe(reader, caller, ref expression))
                    return true;
            }

        failure:
            return false;
        }

        bool TryPipe(in BoaReader reader, in Executor caller, ref ExpressionExecutor expression)
        {
            if (!reader.TryReadChar_match('|'))
                return true;
            else if (!reader.TryReadArgument(out string pipe_cont_name, as_function_argument: false))
                reader.error ??= $"expected command after pipe operator '|'";
            else if (!global_contracts.TryGetValue(pipe_cont_name, out var pipe_cont))
                reader.error ??= $"can not find command with name '{pipe_cont_name}'";
            else
            {
                expression.pipe_next = new ContractExecutor(this, caller, pipe_cont, reader, pipe_previous: expression);
                if (expression.pipe_next.error != null)
                {
                    reader.error ??= expression.pipe_next.error;
                    return false;
                }

                expression = new PipeExecutor(this, caller, expression, expression.pipe_next);

                if (!TryPipe(reader, caller, ref expression) || reader.error != null)
                {
                    reader.error ??= $"could not parse pipe statement";
                    return false;
                }

                return true;
            }

            return false;
        }
    }
}