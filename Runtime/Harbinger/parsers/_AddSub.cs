namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseAddSub(in BoaReader reader, in Executor caller, out ExpressionExecutor expression)
        {
            expression = null;
            if (TryParseTerm(reader, caller, out var term1))
            {
                int read_old = reader.read_i;
                if (reader.TryReadChar_match_out(out char op_char, true, "+-") && !reader.TryReadChar_match(op_char, skippables: null))
                {
                    OperatorsM code = op_char switch
                    {
                        '+' => OperatorsM.add,
                        '-' => OperatorsM.sub,
                        _ => 0,
                    };

                    if (TryParseTerm(reader, caller, out var term2))
                    {
                        ContractExecutor exe = new(this, caller, cmd_math_, reader, parse_arguments: false);
                        exe.args.Add(code);
                        exe.args.Add(term1);
                        exe.args.Add(term2);
                        expression = exe;
                        return true;
                    }
                    else
                    {
                        reader.error ??= $"expected expression after '{op_char}' operator";
                        return false;
                    }
                }
                else
                {
                    reader.read_i = read_old;
                    expression = term1;
                    return true;
                }
            }
            return false;
        }
    }
}