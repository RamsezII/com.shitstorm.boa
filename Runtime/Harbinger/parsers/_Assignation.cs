using System.Collections.Generic;

namespace _BOA_
{
    partial class Harbinger
    {
        internal bool TryParseAssignation(in BoaReader reader, in ScopeNode scope, out ExpressionExecutor assignation)
        {
            assignation = null;
            int read_old = reader.read_i;

            if (!TryParseVariable(reader, scope, out string var_name, out VariableExecutor var_exe))
                goto failure;

            assignation = var_exe;

            if (!TryParseFactorAttribute(reader, scope, ref assignation))
                goto failure;

            List<string> matches = new() { "=", "+=", "-=", "*=", "/=", "&=", "|=", "^=", };

            if (!reader.TryReadString_matches_out(
                out string op_name,
                as_function_argument: false,
                lint: reader.lint_theme.operators,
                ignore_case: true,
                add_to_completions: true,
                skippables: BoaReader._empties_,
                stoppers: " \n\r{}(),;'\"",
                matches: matches)
                )
                goto failure;
            else
            {
                OperatorsM code = op_name switch
                {
                    "+=" => OperatorsM.add,
                    "-=" => OperatorsM.sub,
                    "*=" => OperatorsM.mul,
                    "/=" => OperatorsM.div,
                    "&=" => OperatorsM.and,
                    "|=" => OperatorsM.or,
                    "^=" => OperatorsM.xor,
                    _ => OperatorsM.unknown,
                };

                code |= OperatorsM.assign;

                if (TryParseExpression(reader, scope, false, typeof(object), out var expr))
                {
                    ContractExecutor exe = new(this, scope, cmd_assign_, reader, parse_arguments: false);
                    exe.args.Add(code);
                    exe.args.Add(var_name);
                    exe.arg_0 = expr;
                    assignation = exe;
                    return true;
                }
                else
                {
                    reader.Stderr($"expected expression after '{op_name}' operator.");
                    goto failure;
                }
            }

        failure:
            reader.read_i = read_old;
            assignation = null;
            return false;
        }
    }
}