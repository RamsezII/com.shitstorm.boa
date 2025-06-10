namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseOr(in BoaReader reader, out ContractExecutor expression, out string error)
        {
            expression = null;
            if (TryParseAnd(reader, out var or1, out error))
            {
                if (reader.TryReadMatch(out string op_name, true, true, "or", "||"))
                {
                    if (TryParseAnd(reader, out var or2, out error))
                    {
                        expression = new ContractExecutor(this, cmd_math_, reader, parse_arguments: false);
                        expression.args.Add(OperatorsM.or);
                        expression.args.Add(or1);
                        expression.args.Add(or2);
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
            }
            return false;
        }
    }
}