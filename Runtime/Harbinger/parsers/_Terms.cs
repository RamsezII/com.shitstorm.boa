namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseTerm(in BoaReader reader, in ScopeNode scope, out ExpressionExecutor term, out string error)
        {
            term = null;

            if (!TryParseFactor(reader, scope, out var factor1, out error))
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

                if (TryParseFactor(reader, scope, out var factor2, out error))
                {
                    ContractExecutor exe = new(this, scope, cmd_math_, reader, parse_arguments: false);
                    exe.args.Add(code);
                    exe.args.Add(factor1);
                    exe.args.Add(factor2);
                    term = exe;
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
                term = factor1;
                return true;
            }
        }
    }
}