namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseAddSub(in BoaReader reader, in ScopeNode scope, out ExpressionExecutor expression)
        {
            expression = null;
            if (TryParseTerm(reader, scope, out var term1))
            {
                int read_old = reader.read_i;
                if (reader.TryReadChar_match_out(out char op_symbol, true, "+-"))
                {
                    reader.LintToThisPosition(reader.lint_theme.operators);

                    OperatorsM code = op_symbol switch
                    {
                        '+' => OperatorsM.add,
                        '-' => OperatorsM.sub,
                        _ => 0,
                    };

                    if (TryParseAddSub(reader, scope, out var term2))
                    {
                        ContractExecutor exe = new(this, scope, cmd_math_, reader, parse_arguments: false);
                        exe.args.Add(code);
                        exe.args.Add(term1);
                        exe.args.Add(term2);
                        expression = exe;
                        return true;
                    }
                    else
                    {
                        reader.sig_error ??= $"expected expression after '{op_symbol}' operator";
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