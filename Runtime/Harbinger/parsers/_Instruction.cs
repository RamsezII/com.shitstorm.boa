namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseInstruction(in BoaReader reader, in ScopeNode scope, in bool check_semicolon, out Executor instruction)
        {
            instruction = null;

            if (reader.TryReadChar_match(';', lint: reader.lint_theme.command_separators))
                return true;
            else if (reader.TryReadChar_match('#', lint: reader.lint_theme.comments))
            {
                reader.SkipUntil('\n');
                return true;
            }
            else if (FunctionContract.TryParseFunction(this, reader, scope))
                return true;
            else if (reader.error != null)
                return false;
            else if (TryParseExpression(reader, scope, false, out var expr))
            {
                if (expr is not ContractExecutor contractor || !contractor.contract.no_semicolon_required)
                    if (check_semicolon || reader.strict_syntax)
                        if (!reader.TryReadChar_match(';', lint: reader.lint_theme.command_separators))
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