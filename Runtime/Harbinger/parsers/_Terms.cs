namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseTerm(in BoaReader reader, in ScopeNode scope, out ExpressionExecutor term)
        {
            term = null;

            if (!TryParseUnary(reader, scope, out var expr1))
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

                if (TryParseTerm(reader, scope, out var expr2))
                {
                    ContractExecutor exe = new(this, scope, cmd_math_, reader, parse_arguments: false);
                    exe.args.Add(code);
                    exe.args.Add(expr1);
                    exe.args.Add(expr2);
                    term = exe;
                    return true;
                }
                else
                {
                    reader.sig_error ??= $"expected expression after '{op_char}' operator";
                    return false;
                }
            }
            else
            {
                term = expr1;
                return true;
            }
        }
    }
}