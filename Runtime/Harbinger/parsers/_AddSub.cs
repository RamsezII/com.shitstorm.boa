namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseAddSub(in BoaReader reader, out ContractExecutor expression, out string error)
        {
            expression = null;
            if (TryParseTerm(reader, out var term1, out error))
            {
                if (reader.TryReadMatch(out char op_char, true, "+-"))
                {
                    OperatorsM code = op_char switch
                    {
                        '+' => OperatorsM.add,
                        '-' => OperatorsM.sub,
                        _ => 0,
                    };

                    if (TryParseTerm(reader, out var term2, out error))
                    {
                        expression = new ContractExecutor(this, cmd_math_, reader, parse_arguments: false);
                        expression.args.Add(code);
                        expression.args.Add(term1);
                        expression.args.Add(term2);
                        return true;
                    }
                    else
                    {
                        error ??= $"expected term after '{op_char}' operator";
                        return false;
                    }
                }
                else
                {
                    expression = term1;
                    return true;
                }
            }
            return false;
        }
    }
}