namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseAnd(in BoaReader reader, in ScopeNode scope, out ExpressionExecutor expression)
        {
            expression = null;
            if (TryParseComparison(reader, scope, out var and1))
                if (reader.TryReadString_match_out(out string op_name, as_function_argument: false, lint: reader.lint_theme.keywords, match: "and"))
                {
                    if (TryParseAnd(reader, scope, out var and2))
                    {
                        ContractExecutor exe = new(this, scope, cmd_math_, reader, parse_arguments: false);
                        exe.args.Add(OperatorsM.and);
                        exe.args.Add(and1);
                        exe.args.Add(and2);
                        expression = exe;
                        return true;
                    }
                    else
                    {
                        reader.sig_error ??= $"expected expression after '{op_name}' operator";
                        return false;
                    }
                }
                else
                {
                    expression = and1;
                    return true;
                }
            return false;
        }
    }
}