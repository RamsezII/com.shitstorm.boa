namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseAnd(in BoaReader reader, out ContractExecutor expression, out string error)
        {
            expression = null;
            if (TryParseComparison(reader, out var and1, out error))
                if (reader.TryReadMatch(out string op_name, "and"))
                {
                    if (TryParseComparison(reader, out var and2, out error))
                    {
                        expression = new ContractExecutor(this, cmd_math_, reader, parse_arguments: false);
                        expression.args.Add(OperatorsM.and);
                        expression.args.Add(and1);
                        expression.args.Add(and2);
                        return true;
                    }
                    else
                    {
                        error ??= $"expected expression after '{op_name}' operator";
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