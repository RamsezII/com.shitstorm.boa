namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseFactor(in BoaReader reader, out ExpressionExecutor factor, out string error)
        {
            factor = null;
            error = null;

            if (reader.TryReadMatch(out char unary_operator, true, "+-!"))
            {
                if (TryParseFactor(reader, out var sub_factor, out error))
                {
                    OperatorsM code = unary_operator switch
                    {
                        '+' => OperatorsM.add,
                        '-' => OperatorsM.sub,
                        '!' => OperatorsM.not,
                        _ => 0,
                    };

                    ContractExecutor exe = new(this, cmd_unary_, reader, parse_arguments: false);
                    exe.args.Add(code);
                    exe.args.Add(sub_factor);
                    factor = exe;
                    return true;
                }
                else
                {
                    error ??= $"expected factor after '{unary_operator}'";
                    return false;
                }
            }

            if (error == null)
                if (reader.TryReadMatch('('))
                    if (!TryParseExpression(reader, false, out factor, out error))
                    {
                        error ??= "expected expression inside factor parenthesis";
                        return false;
                    }
                    else if (!reader.TryReadMatch(')'))
                    {
                        error ??= $"expected closing parenthesis ')'";
                        return false;
                    }
                    else
                        return true;

            if (error == null)
                if (TryParseString(reader, out string str, out error))
                {
                    factor = new LiteralExecutor(this, literal: str);
                    return true;
                }

            if (error == null)
                if (reader.TryReadArgument(out string arg, out error, as_function_argument: false))
                    if (global_contracts.TryGetValue(arg, out var contract))
                    {
                        factor = new ContractExecutor(this, contract, reader);
                        if (factor.error != null)
                        {
                            error = factor.error;
                            return false;
                        }
                        return true;
                    }
                    else if (global_variables.TryGetValue(arg, out var variable))
                    {
                        factor = new VariableExecutor(this, variable);
                        return true;
                    }
                    else
                        switch (arg.ToLower())
                        {
                            case "true":
                                factor = new LiteralExecutor(this, literal: true);
                                return true;

                            case "false":
                                factor = new LiteralExecutor(this, literal: false);
                                return true;

                            default:
                                if (int.TryParse(arg, out int _int))
                                    factor = new LiteralExecutor(this, literal: _int);
                                else if (Util.TryParseFloat(arg, out float _float))
                                    factor = new LiteralExecutor(this, literal: _float);
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