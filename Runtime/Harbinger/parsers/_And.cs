namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseAnd(in BoaReader reader, in Executor caller, out ExpressionExecutor expression)
        {
            expression = null;
            if (TryParseComparison(reader, caller, out var and1))
                if (reader.TryReadString_match_out(out string op_name, lint: reader.lint_theme.keywords, match: "and"))
                {
                    if (TryParseComparison(reader, caller, out var and2))
                    {
                        ContractExecutor exe = new(this, caller, cmd_math_, reader, parse_arguments: false);
                        exe.args.Add(OperatorsM.and);
                        exe.args.Add(and1);
                        exe.args.Add(and2);
                        expression = exe;
                        return true;
                    }
                    else
                    {
                        reader.error ??= $"expected expression after '{op_name}' operator";
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