namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseAssignation(in BoaReader reader, out ContractExecutor assignation, out string error)
        {
            assignation = null;
            int read_i = reader.read_i;

            if (reader.TryReadArgument(out string varname, out error, as_function_argument: false))
                if (global_variables.TryGetValue(varname, out var variable))
                    if (reader.TryReadMatch(out string op_name, true, skippables: BoaReader._empties_, stoppers: " \n\r{}(),;'\"", "=", "+=", "-=", "*=", "/="))
                    {
                        OperatorsM code = op_name switch
                        {
                            "=" => OperatorsM.eq,
                            "+=" => OperatorsM.add,
                            "-=" => OperatorsM.sub,
                            "*=" => OperatorsM.mul,
                            "/=" => OperatorsM.div,
                            _ => OperatorsM.unknown,
                        };

                        code |= OperatorsM.assign;

                        if (TryParseExpression(reader, false, out var expr, out error))
                        {
                            assignation = new ContractExecutor(this, cmd_assign_, reader, parse_arguments: false);
                            assignation.args.Add(code);
                            assignation.args.Add(variable);
                            assignation.args.Add(expr);
                            return true;
                        }
                        else
                        {
                            error ??= $"expected expression after '{op_name}' operator";
                            return false;
                        }
                    }

            reader.read_i = read_i;
            return false;
        }
    }
}