namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseFactor(in BoaReader reader, out ContractExecutor factor, out string error)
        {
            error = null;
            factor = null;

            if (reader.TryReadChar('('))
                if (TryParseExpression(reader, out factor, out error))
                    if (reader.TryReadChar(')'))
                        return true;
                    else
                    {
                        error ??= "expected expression inside parentheses";
                        return false;
                    }
                else
                {
                    error ??= $"expected closing parenthesis ')'";
                    return false;
                }

            if (reader.TryReadArgument(out string arg))
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
                                factor.args.Add(arg);
                            return true;
                    }
                }

            return false;
        }
    }
}