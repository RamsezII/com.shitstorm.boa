namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseAssignation(in BoaReader reader, in Executor parent, out ExpressionExecutor assignation, out string error)
        {
            assignation = null;
            int read_old = reader.read_i;

            if (reader.TryReadArgument(out string varname, out error, as_function_argument: false))
                if (parent.TryGetVariable(varname, out var variable))
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

                        if (TryParseExpression(reader, parent, false, out var expr, out error))
                        {
                            ContractExecutor exe = new(this, parent, cmd_assign_, reader, parse_arguments: false);
                            exe.args.Add(code);
                            exe.args.Add(variable);
                            exe.args.Add(expr);
                            assignation = exe;
                            return true;
                        }
                        else
                        {
                            error ??= $"expected expression after '{op_name}' operator";
                            return false;
                        }
                    }

            reader.read_i = read_old;
            return false;
        }
    }
}