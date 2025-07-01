namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseUnary(in BoaReader reader, in ScopeNode scope, out ExpressionExecutor expression)
        {
            expression = null;

            if (reader.TryReadChar_matches_out(out char unary_operator, true, "+-!"))
            {
                UnaryExecutor.Operators code = unary_operator switch
                {
                    '+' => UnaryExecutor.Operators.Add,
                    '-' => UnaryExecutor.Operators.Sub,
                    '!' => UnaryExecutor.Operators.Not,
                    _ => 0,
                };

                switch (code)
                {
                    case UnaryExecutor.Operators.Add:
                    case UnaryExecutor.Operators.Sub:
                        {
                            int read_old = reader.read_i;
                            if (reader.TryReadChar_match(unary_operator, reader.lint_theme.operators, skippables: null))
                            {
                                if (!reader.TryReadArgument(out string var_name, false, reader.lint_theme.variables, skippables: null))
                                    reader.Stderr($"expected variable after increment operator '{unary_operator}{unary_operator}'.");
                                else if (!scope.TryGetVariable(var_name, out _))
                                    reader.Stderr($"no variable named '{var_name}'.");
                                else
                                {
                                    expression = new IncrementExecutor(this, scope, var_name, code switch
                                    {
                                        UnaryExecutor.Operators.Add => IncrementExecutor.Operators.AddBefore,
                                        UnaryExecutor.Operators.Sub => IncrementExecutor.Operators.SubBefore,
                                        _ => 0,
                                    });
                                    return reader.sig_error == null;
                                }
                                reader.read_i = read_old;
                                return false;
                            }
                        }
                        break;
                }

                if (TryParseFactor_with_attribute(reader, scope, out expression))
                {
                    expression = new UnaryExecutor(this, scope, expression, code);
                    return true;
                }
                else
                {
                    reader.Stderr($"expected factor after '{unary_operator}'.");
                    return false;
                }
            }

            if (TryParseFactor_with_attribute(reader, scope, out var list))
            {
                if (reader.TryReadChar_match('['))
                {
                    reader.LintOpeningBraquet();
                    if (!TryParseExpression(reader, scope, false, typeof(int), out var index))
                        reader.Stderr($"expected expression inside index accessor.");
                    else if (!reader.TryReadChar_match(']', lint: reader.CloseBraquetLint()))
                        reader.Stderr($"expected closing braquet ']'.");
                    else
                    {
                        expression = new SubArrayExecutor(this, scope, list, index);
                        return reader.sig_error == null;
                    }
                    return false;
                }
                expression = list;
                return true;
            }

            return false;
        }
    }
}