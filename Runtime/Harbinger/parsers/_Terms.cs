namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseTerm(in BoaReader reader, in Executor caller, out ExpressionExecutor term)
        {
            term = null;

            if (!TryParseFactor(reader, caller, out var factor1))
                return false;

            if (reader.TryReadChar_match_out(out char op_char, true, "*/%"))
            {
                OperatorsM code = op_char switch
                {
                    '*' => OperatorsM.mul,
                    '/' => OperatorsM.div,
                    '%' => OperatorsM.mod,
                    _ => 0,
                };

                if (TryParseFactor(reader, caller, out var factor2))
                {
                    ContractExecutor exe = new(this, caller, cmd_math_, reader, parse_arguments: false);
                    exe.args.Add(code);
                    exe.args.Add(factor1);
                    exe.args.Add(factor2);
                    term = exe;
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
                term = factor1;
                return true;
            }
        }
    }
}