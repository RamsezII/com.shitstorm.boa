namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseComparison(in BoaReader reader, out ContractExecutor expression, out string error)
        {
            expression = null;
            if (TryParseAddSub(reader, out var addsub1, out error))
            {
                if (reader.TryReadChar(out char op_char, "<>="))
                {
                    OperatorsM code = op_char switch
                    {
                        '<' => OperatorsM.lt,
                        '>' => OperatorsM.gt,
                        '=' => OperatorsM.eq,
                        _ => 0,
                    };

                    if (reader.TryReadChar('=') && code != 0)
                        code |= OperatorsM.eq;

                    if (TryParseAddSub(reader, out var addsub2, out error))
                    {
                        expression = new ContractExecutor(this, cmd_math_, reader, parse_arguments: false);
                        expression.args.Add(code);
                        expression.args.Add(addsub1);
                        expression.args.Add(addsub2);
                        return true;
                    }
                    else
                    {
                        error ??= $"expected comparison after '{op_char}' operator";
                        return false;
                    }
                }
                else
                {
                    expression = addsub1;
                    return true;
                }
            }
            return false;
        }
    }
}