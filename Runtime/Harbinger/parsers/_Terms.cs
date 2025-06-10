namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseTerm(in BoaReader reader, out ContractExecutor term, out string error)
        {
            term = null;

            if (!TryParseFactor(reader, out var factor1, out error))
                return false;

            if (reader.TryReadChar(out char op_char, "*/%", ignore_case: true, skip_empties: true))
            {
                OperatorsM code = op_char switch
                {
                    '*' => OperatorsM.mul,
                    '/' => OperatorsM.div,
                    '%' => OperatorsM.mod,
                    _ => 0,
                };

                if (TryParseFactor(reader, out var factor2, out error))
                {
                    term = new ContractExecutor(this, cmd_math_, reader, parse_arguments: false);
                    term.args.Add(code);
                    term.args.Add(factor1);
                    term.args.Add(factor2);
                    return true;
                }
                else
                {
                    error ??= $"expected term after '{op_char}' operator";
                    return false;
                }
            }
            else
            {
                term = factor1;
                return true;
            }
        }
    }
}