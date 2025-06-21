namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseExpression(in BoaReader reader, in ScopeNode scope, in bool allow_argument_syntax, out ExpressionExecutor expression)
        {
            if (TryParseAssignation(reader, scope, out expression) || reader.sig_error == null && TryParseOr(reader, scope, out expression))
            {
                if (allow_argument_syntax && !reader.TryReadChar_match(',', lint: reader.lint_theme.argument_coma) && !reader.TryPeekChar_match(')', out _))
                    if (reader.strict_syntax)
                    {
                        reader.sig_error ??= $"expected ',' or ')' after expression";
                        if (expression is ContractExecutor cont)
                            reader.sig_error += $" ('{cont.contract.name}')";
                        else
                            reader.sig_error += $" ('{expression.GetType()}')";
                        goto failure;
                    }

                if (reader.TryReadChar_match('?', lint: reader.lint_theme.operators))
                {
                    var cond = expression;
                    if (!TryParseExpression(reader, scope, false, out var _if))
                        reader.sig_error ??= $"expected expression after ternary operator '?'";
                    else if (!reader.TryReadChar_match(':', lint: reader.lint_theme.operators))
                        reader.sig_error ??= $"expected ternary operator delimiter ':'";
                    else if (!TryParseExpression(reader, scope, false, out var _else))
                        reader.sig_error ??= $"expected second expression after ternary operator ':'";
                    else
                    {
                        expression = new TernaryOpExecutor(this, scope, cond, _if, _else);
                        if (reader.sig_error != null)
                            return false;
                    }
                }

                if (TryPipe(reader, scope, ref expression))
                    return true;
            }

        failure:
            return false;
        }

        bool TryPipe(in BoaReader reader, in ScopeNode scope, ref ExpressionExecutor expression)
        {
            if (!reader.TryReadChar_match('|', lint: reader.lint_theme.operators))
                return true;
            else if (!reader.TryReadArgument(out string pipe_cont_name, lint: reader.lint_theme.contracts, as_function_argument: false))
                reader.sig_error ??= $"expected command after pipe operator '|'";
            else if (!global_contracts.TryGetValue(pipe_cont_name, out var pipe_cont))
                reader.sig_error ??= $"can not find command with name '{pipe_cont_name}'";
            else
            {
                expression.pipe_next = new ContractExecutor(this, scope, pipe_cont, reader, pipe_previous: expression);
                if (reader.sig_error != null)
                    return false;

                expression = new PipeExecutor(this, scope, expression, expression.pipe_next);

                if (!TryPipe(reader, scope, ref expression) || reader.sig_error != null)
                {
                    reader.sig_error ??= $"could not parse pipe statement";
                    return false;
                }

                return true;
            }

            return false;
        }
    }
}