namespace _BOA_
{
    partial class Harbinger
    {
        public bool TryParseExpression(in BoaReader reader, in ScopeNode scope, in bool read_as_argument, out ExpressionExecutor expression)
        {
            if (TryParseAssignation(reader, scope, out expression) || reader.sig_error == null && TryParseOr(reader, scope, out expression))
            {
                if (read_as_argument && !reader.TryReadChar_match(',', lint: reader.lint_theme.argument_coma) && !reader.TryPeekChar_match(')', out _))
                    if (reader.strict_syntax)
                    {
                        reader.Stderr($"expected ',' or ')' after expression.");
                        if (expression is ContractExecutor cont)
                            reader.sig_error += $" ('{cont.contract.name}')";
                        else
                            reader.sig_error += $" ('{expression.GetType()}')";
                        return false;
                    }

                if (reader.TryReadChar_match('?', lint: reader.lint_theme.operators))
                {
                    var cond = expression;
                    if (!TryParseExpression(reader, scope, false, out var _if))
                        reader.Stderr($"expected expression after ternary operator '?'.");
                    else if (!reader.TryReadChar_match(':', lint: reader.lint_theme.operators))
                        reader.Stderr($"expected ternary operator delimiter ':'.");
                    else if (!TryParseExpression(reader, scope, false, out var _else))
                        reader.Stderr($"expected second expression after ternary operator ':'.");
                    else
                    {
                        expression = new TernaryOpExecutor(this, scope, cond, _if, _else);
                        if (reader.sig_error != null)
                            return false;
                    }
                }

                if (!TryPipe(reader, scope, ref expression))
                {
                    reader.Stderr($"could not parse pipe statement.");
                    return false;
                }

                if (reader.TryReadChar_match('.', lint: reader.lint_theme.point))
                {
                    reader.Stderr($"type: {{{expression?.OutputType()}}}");
                    return false;
                }

                return true;
            }
            return false;
        }

        bool TryPipe(in BoaReader reader, in ScopeNode scope, ref ExpressionExecutor current_expression)
        {
            if (!reader.TryReadChar_match('|', lint: reader.lint_theme.operators))
                return true;
            else if (!TryParseMethod(reader, scope, current_expression, out var cont_pipe))
                reader.Stderr($"expected method after pipe symbol '|'.");
            else
            {
                current_expression = new PipeExecutor(this, scope, current_expression);

                if (!TryPipe(reader, scope, ref current_expression) || reader.sig_error != null)
                {
                    reader.Stderr($"could not parse pipe statement.");
                    return false;
                }

                return true;
            }

            return false;
        }
    }
}