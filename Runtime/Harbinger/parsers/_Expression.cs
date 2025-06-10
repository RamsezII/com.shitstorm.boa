namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseExpression(BoaReader reader, out ContractExecutor expression, out string error)
        {
            if (TryParseAssignation(reader, out expression, out error))
                return true;
            else if (TryParseOr(reader, out expression, out error))
                return true;
            return false;
        }
    }
}