namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseComparison(in BoaReader reader, in Executor parent, out ExpressionExecutor expression, out string error)
        {
            expression = null;
            if (TryParseAddSub(reader, parent, out var addsub1, out error))
            {
                if (reader.TryReadChar_match_out(out char op_char, true, "<>="))
                {
                    OperatorsM code = op_char switch
                    {
                        '<' => OperatorsM.lt,
                        '>' => OperatorsM.gt,
                        '=' => OperatorsM.eq,
                        _ => 0,
                    };

                    if (reader.TryReadChar_match('=') && code != 0)
                        code |= OperatorsM.eq;

                    if (TryParseAddSub(reader, parent, out var addsub2, out error))
                    {
                        ContractExecutor exe = new(this, parent, cmd_math_, reader, parse_arguments: false);
                        exe.args.Add(code);
                        exe.args.Add(addsub1);
                        exe.args.Add(addsub2);
                        expression = exe;
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