namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseFactor(in BoaReader reader, in ScopeNode scope, out ExpressionExecutor factor, out string error)
        {
            factor = null;
            error = null;

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
                            if (reader.TryReadChar_match(unary_operator, skippables: null))
                            {
                                if (!reader.TryReadArgument(out string varname, out error, skippables: null))
                                    error ??= $"expected variable after increment operator '{unary_operator}{unary_operator}'";
                                else if (!scope.TryGetVariable(varname, out var variable))
                                    error ??= $"no variable named '{varname}'";
                                else
                                {
                                    factor = new IncrementExecutor(this, scope, variable, code switch
                                    {
                                        UnaryExecutor.Operators.Add => IncrementExecutor.Operators.AddBefore,
                                        UnaryExecutor.Operators.Sub => IncrementExecutor.Operators.SubBefore,
                                        _ => 0,
                                    });
                                    if (factor.error != null)
                                    {
                                        error = factor.error;
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

                if (TryParseFactor(reader, scope, out var sub_factor, out error))
                {

                    factor = new UnaryExecutor(this, scope, sub_factor, code);
                    return true;
                }
                else
                {
                    error ??= $"expected factor after '{unary_operator}'";
                    return false;
                }
            }

            if (error == null)
                if (reader.TryReadChar_match('('))
                    if (!TryParseExpression(reader, scope, false, out factor, out error))
                    {
                        error ??= "expected expression inside factor parenthesis";
                        return false;
                    }
                    else if (!reader.TryReadChar_match(')'))
                    {
                        error ??= $"expected closing parenthesis ')' after factor {factor.ToLog}";
                        --reader.read_i;
                        return false;
                    }
                    else
                        return true;

            if (error == null)
                if (TryParseString(reader, out string str, out error))
                {
                    factor = new LiteralExecutor(this, scope, literal: str);
                    return true;
                }

            if (error == null)
                if (reader.TryReadArgument(out string arg, out error, as_function_argument: false))
                    if (global_contracts.TryGetValue(arg, out var contract))
                    {
                        factor = new ContractExecutor(this, scope, contract, reader);
                        if (factor.error != null)
                        {
                            error = factor.error;
                            return false;
                        }
                        return true;
                    }
                    else if (scope.TryGetFunction(arg, out var function))
                    {
                        factor = new FunctionExecutor(this, function.scope, function, reader);
                        if (factor.error != null)
                        {
                            error ??= factor.error;
                            return false;
                        }
                        return true;
                    }
                    else if (scope.TryGetVariable(arg, out var variable))
                    {
                        factor = new VariableExecutor(this, scope, variable);
                        return true;
                    }
                    else
                        switch (arg.ToLower())
                        {
                            case "true":
                                factor = new LiteralExecutor(this, scope, literal: true);
                                return true;

                            case "false":
                                factor = new LiteralExecutor(this, scope, literal: false);
                                return true;

                            default:
                                if (int.TryParse(arg, out int _int))
                                    factor = new LiteralExecutor(this, scope, literal: _int);
                                else if (Util.TryParseFloat(arg, out float _float))
                                    factor = new LiteralExecutor(this, scope, literal: _float);
                                else
                                {
                                    error ??= $"unrecognized object : '{arg}'";
                                    return false;
                                }
                                return true;
                        }

            return false;
        }
    }
}