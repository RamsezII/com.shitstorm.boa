namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseOr(in BoaReader reader, in Executor parent, out ExpressionExecutor expression, out string error)
        {
            expression = null;
            if (TryParseAnd(reader, parent, out var or1, out error))
                if (reader.TryReadMatch(out string op_name, "or"))
                {
                    if (TryParseAnd(reader, parent, out var or2, out error))
                    {
                        ContractExecutor exe = new(this, parent, cmd_math_, reader, parse_arguments: false);
                        exe.args.Add(OperatorsM.or);
                        exe.args.Add(or1);
                        exe.args.Add(or2);
                        expression = exe;
                        return true;
                    }
                    else
                    {
                        error ??= $"expected right operand for '{op_name}' operator";
                        return false;
                    }
                }
                else
                {
                    expression = or1;
                    return true;
                }
            return false;
        }
    }
}