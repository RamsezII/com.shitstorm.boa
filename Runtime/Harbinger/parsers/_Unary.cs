namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseUnary(in BoaReader reader, in Executor caller, out ExpressionExecutor expression)
        {
            expression = null;

            if (reader.TryReadChar_match_out(out char unary_operator, true, "+-!"))
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
                                if (!reader.TryReadArgument(out string varname, false, reader.lint_theme.variables, skippables: null))
                                    reader.error ??= $"expected variable after increment operator '{unary_operator}{unary_operator}'";
                                else if (!caller._variables.TryGet(varname, out var variable))
                                    reader.error ??= $"no variable named '{varname}'";
                                else
                                {
                                    expression = new IncrementExecutor(this, caller, variable, code switch
                                    {
                                        UnaryExecutor.Operators.Add => IncrementExecutor.Operators.AddBefore,
                                        UnaryExecutor.Operators.Sub => IncrementExecutor.Operators.SubBefore,
                                        _ => 0,
                                    });
                                    if (expression.error != null)
                                    {
                                        reader.error = expression.error;
                                        return false;
                                    }
                                    return true;
                                }
                                reader.read_i = read_old;
                                return false;
                            }
                        }
                        break;
                }

                if (TryParseFactor(reader, caller, out var factor))
                {
                    expression = new UnaryExecutor(this, caller, factor, code);
                    return true;
                }
                else
                {
                    reader.error ??= $"expected factor after '{unary_operator}'";
                    return false;
                }
            }

            if (TryParseFactor(reader, caller, out var list))
            {
                if (reader.TryReadChar_match('[', lint: reader.OpenBraquetLint()))
                {
                    if (!TryParseExpression(reader, caller, false, out var index))
                        reader.error ??= $"expected expression inside index accessor";
                    else if (!reader.TryReadChar_match(']', lint: reader.CloseBraquetLint()))
                        reader.error ??= $"expected closing braquet ']'";
                    else
                    {
                        expression = new SubArrayExecutor(this, caller, list, index);
                        if (expression.error != null)
                        {
                            reader.error ??= expression.error;
                            return false;
                        }
                        return true;
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