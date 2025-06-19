namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseAssignation(in BoaReader reader, in ScopeNode scope, out ExpressionExecutor assignation)
        {
            assignation = null;
            int read_old = reader.read_i;

            if (TryParseVariable(reader, scope, out var var_exe))
                if (reader.TryReadString_matches_out(
                    out string op_name,
                    as_function_argument: false,
                    lint: reader.lint_theme.operators,
                    ignore_case: true,
                    add_to_completions: true,
                    skippables: BoaReader._empties_,
                    stoppers: " \n\r{}(),;'\"",
                    "=", "+=", "-=", "*=", "/=")
                    )
                {
                    OperatorsM code = op_name switch
                    {
                        "==" => OperatorsM.eq,
                        "+=" => OperatorsM.add,
                        "-=" => OperatorsM.sub,
                        "*=" => OperatorsM.mul,
                        "/=" => OperatorsM.div,
                        _ => OperatorsM.unknown,
                    };

                    code |= OperatorsM.assign;

                    if (TryParseExpression(reader, scope, false, out var expr))
                    {
                        ContractExecutor exe = new(this, scope, cmd_assign_, reader, parse_arguments: false);
                        exe.args.Add(code);
                        exe.args.Add(var_exe);
                        exe.arg_0 = expr;
                        assignation = exe;
                        return true;
                    }
                    else
                    {
                        reader.error ??= $"expected expression after '{op_name}' operator";
                        return false;
                    }
                }

            reader.read_i = read_old;
            return false;
        }
    }
}