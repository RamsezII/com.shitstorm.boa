namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseExpression(BoaReader reader, in bool as_function_argument, out ContractExecutor expression, out string error)
        {
            if (TryParseAssignation(reader, out expression, out error) || TryParseOr(reader, out expression, out error))
            {
                if (!as_function_argument || !reader.strict_syntax || reader.TryReadMatch(',') || reader.TryPeekSpecific(')'))
                    return true;
                error = $"expected ',' or ')' after expression '{expression.contract.name}'";
            }
            return false;
        }
    }
}