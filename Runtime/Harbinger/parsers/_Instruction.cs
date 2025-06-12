namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseInstruction(in BoaReader reader, in bool check_for_comma, out Executor instruction, out string error)
        {
            instruction = null;
            error = null;

            if (reader.HasNext())
                if (reader.TryReadChar(';'))
                {
                    instruction = new ContractExecutor(this, null, reader);
                    return true;
                }
                else if (reader.TryReadChar('#'))
                {
                    reader.SkipUntil('\n');
                    instruction = new ContractExecutor(this, null, reader);
                    return true;
                }
                else if (TryParseExpression(reader, false, out var expr, out error))
                {
                    if (expr is not ContractExecutor contractor || !contractor.contract.no_semicolon_required)
                        if ((check_for_comma || reader.IsScript) && !reader.TryReadChar(';'))
                            if (check_for_comma)
                            {
                                error ??= $"missing ';' at the end of instruction";
                                return false;
                            }

                    instruction = expr;
                    return true;
                }

            return false;
        }
    }
}