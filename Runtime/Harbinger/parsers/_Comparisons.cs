namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseComparison(in BoaReader reader, in ScopeNode scope, out ExpressionExecutor expression)
        {
            expression = null;
            if (TryParseAddSub(reader, scope, out var addsub1))
            {
                if (reader.TryReadChar_matches_out(out char op_char, true, "<>="))
                {
                    OperatorsM code = op_char switch
                    {
                        '<' => OperatorsM.lt,
                        '>' => OperatorsM.gt,
                        '=' => OperatorsM.eq,
                        _ => 0,
                    };

                    if (reader.TryReadChar_match('=', lint: reader.lint_theme.operators, skippables: null) && code != 0)
                        code |= OperatorsM.eq;

                    reader.LintToThisPosition(reader.lint_theme.operators, true);

                    if (TryParseAddSub(reader, scope, out var addsub2))
                    {
                        ContractExecutor exe = new(this, scope, cmd_math_, reader, parse_arguments: false);
                        exe.args.Add(code);
                        exe.args.Add(addsub1);
                        exe.args.Add(addsub2);
                        expression = exe;
                        return true;
                    }
                    else
                    {
                        reader.Stderr($"expected expression after '{op_char}' operator.");
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