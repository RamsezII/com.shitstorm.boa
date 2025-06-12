namespace _BOA_
{
    partial class Harbinger
    {
        internal static readonly Contract
            cmd_literal = new("literal", function_style_arguments: false, action: static exe => exe.args[0]),
            cmd_variable = new("variable", function_style_arguments: false, action: static exe => ((BoaVar)exe.args[0]).value);

        //----------------------------------------------------------------------------------------------------------

        internal bool TryParseFactor(in BoaReader reader, out ContractExecutor factor, out string error)
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

                    factor = new(this, cmd_unary_, reader, parse_arguments: false);
                    factor.args.Add(code);
                    factor.args.Add(sub_factor);
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
                        error ??= "expected expression inside parentheses";
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
                    factor = new(this, cmd_literal, reader, parse_arguments: false);
                    factor.args.Add(str);
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
                        factor = new(this, cmd_variable, reader, parse_arguments: false);
                        factor.args.Add(variable);
                        return true;
                    }
                    else
                    {
                        factor = new(this, cmd_literal, reader, parse_arguments: false);
                        string lower = arg.ToLower();
                        switch (lower)
                        {
                            case "true":
                                factor.args.Add(true);
                                return true;

                            case "false":
                                factor.args.Add(false);
                                return true;

                            default:
                                if (int.TryParse(arg, out int _int))
                                    factor.args.Add(_int);
                                else if (Util.TryParseFloat(arg, out float _float))
                                    factor.args.Add(_float);
                                else
                                {
                                    factor = null;
                                    error ??= $"unrecognized object : '{arg}'";
                                    return false;
                                }
                                return true;
                        }
                    }

            return false;
        }
    }
}