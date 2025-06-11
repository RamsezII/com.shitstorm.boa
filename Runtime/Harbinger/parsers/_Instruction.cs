namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseInstruction(in BoaReader reader, out Executor instruction, out string error)
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
                else if (TryParseExpression(reader, out var expr, out error))
                {
                    if (reader.IsScript && !reader.TryReadChar(';'))
                    {
                        error ??= $"missing ';' at the end of instruction ({reader.GetType()}.{nameof(reader.last_arg)}: {reader.last_arg})";
                        return false;
                    }

                    instruction = expr;
                    return true;
                }

            return false;
        }
    }
}