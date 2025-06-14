namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseInstruction(in BoaReader reader, in Executor caller, in bool check_semicolon, out Executor instruction)
        {
            instruction = null;

            if (reader.TryReadChar_match(';'))
                return true;
            else if (reader.TryReadChar_match('#'))
            {
                reader.SkipUntil('\n');
                return true;
            }
            else if (FunctionContract.TryParseFunction(reader, caller))
                return true;
            else if (reader.error != null)
                return false;
            else if (TryParseExpression(reader, caller, false, out var expr))
            {
                if (expr is not ContractExecutor contractor || !contractor.contract.no_semicolon_required)
                    if (check_semicolon || reader.strict_syntax)
                        if (!reader.TryReadChar_match(';'))
                            if (check_semicolon && reader.strict_syntax)
                            {
                                reader.error ??= $"missing ';' at the end of instruction";
                                return false;
                            }

                instruction = expr;
                return true;
            }

            return false;
        }
    }
}