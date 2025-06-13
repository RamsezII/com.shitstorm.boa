namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseAddSub(in BoaReader reader, out ExpressionExecutor expression, out string error)
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
                        ContractExecutor exe = new(this, cmd_math_, reader, parse_arguments: false);
                        exe.args.Add(code);
                        exe.args.Add(term1);
                        exe.args.Add(term2);
                        expression = exe;
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