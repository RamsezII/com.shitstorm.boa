namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseInstruction(in BoaReader reader, in ScopeNode scope, in bool check_semicolon, out Executor instruction, out string error)
        {
            instruction = null;
            error = null;

            if (reader.TryReadChar_match(';'))
                return true;
            else if (reader.TryReadChar_match('#'))
            {
                reader.SkipUntil('\n');
                return true;
            }
            else if (FunctionContract.TryParseFunction(this, scope, reader, out error))
                return true;
            else if (error != null)
                return false;
            else if (TryParseExpression(reader, scope, false, out var expr, out error))
            {
                if (expr is not ContractExecutor contractor || !contractor.contract.no_semicolon_required)
                    if (check_semicolon || reader.strict_syntax)
                        if (!reader.TryReadChar_match(';'))
                            if (check_semicolon && reader.strict_syntax)
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