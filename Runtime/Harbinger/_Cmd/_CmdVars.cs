namespace _BOA_
{
    partial class Harbinger
    {
        static Contract
            cmd_literal = new("literal", typeof(object), action: static exe => exe.args[0]),
            cmd_variable = new("variable", typeof(object), action: static exe => ((Variable<object>)exe.args[0]).value),
            cmd_ass_,
            cmd_ass_add_,
            cmd_ass_sub_,
            cmd_ass_and_,
            cmd_ass_or_,
            cmd_eq_,
            cmd_neq_,
            cmd_grt_,
            cmd_eq_grt_,
            cmd_less_,
            cmd_eq_less_,
            cmd_add_,
            cmd_sub_,
            cmd_mult_,
            cmd_div_,
            cmd_div_int,
            cmd_mod_,
            cmd_and_,
            cmd_not_,
            cmd_or_,
            cmd_xor_;

        //----------------------------------------------------------------------------------------------------------

        static bool TryGetOperator(
            in BoaReader reader, out string symbol, out Contract cmd,
            in bool op_assign = false,
            in bool op_or = false,
            in bool op_and = false,
            in bool op_compare = false,
            in bool op_expr = false,
            in bool op_terms = false)
        {
            symbol = string.Empty;
            cmd = null;

            if (reader.TryReadChar(out char c_oper, "=+-*/%<>&|", true))
            {
                symbol = c_oper.ToString();
                switch (c_oper)
                {
                    case '=':
                        if (reader.TryReadChar('='))
                        {
                            symbol += "=";
                            if (op_compare)
                                cmd = cmd_eq_;
                        }
                        else if (op_assign)
                            cmd = cmd_ass_;
                        break;

                    case '>':
                    case '<':
                        if (op_compare)
                            if (reader.TryReadChar('='))
                            {
                                symbol += "=";
                                cmd = c_oper switch
                                {
                                    '>' => cmd_eq_grt_,
                                    '<' => cmd_eq_less_,
                                    _ => null,
                                };
                            }
                            else
                                cmd = c_oper switch
                                {
                                    '>' => cmd_grt_,
                                    '<' => cmd_less_,
                                    _ => null,
                                };
                        break;

                    case '+':
                    case '-':
                    case '*':
                    case '/':
                    case '%':
                        if (c_oper switch
                        {
                            '+' or '-' => op_expr,
                            '*' or '/' or '%' => op_terms,
                            _ => false,
                        })
                            if (reader.TryReadChar('='))
                            {
                                symbol += "=";
                                cmd = c_oper switch
                                {
                                    '+' => cmd_ass_add_,
                                    '-' => cmd_ass_sub_,
                                    '*' => null,
                                    '/' => null,
                                    '%' => null,
                                    _ => null,
                                };
                            }
                            else
                                cmd = c_oper switch
                                {
                                    '+' => cmd_add_,
                                    '-' => cmd_sub_,
                                    '*' => cmd_mult_,
                                    '/' => cmd_div_,
                                    '%' => cmd_mod_,
                                    _ => null,
                                };
                        break;

                    case '&':
                    case '|':
                        if (reader.TryReadChar(c_oper))
                            if (reader.TryReadChar('='))
                            {
                                symbol = $"{c_oper}{c_oper}=";
                                if (op_assign)
                                    cmd = c_oper switch
                                    {
                                        '&' => cmd_ass_and_,
                                        '|' => cmd_ass_or_,
                                        _ => null,
                                    };
                            }
                            else if (c_oper switch
                            {
                                '&' => op_and,
                                '|' => op_or,
                                _ => false,
                            })
                            {
                                symbol = $"{c_oper}{c_oper}";
                                cmd = c_oper switch
                                {
                                    '&' => cmd_and_,
                                    '|' => cmd_or_,
                                    _ => null,
                                };
                            }
                        break;
                }
            }

            if (cmd == null)
                symbol = string.Empty;

            return cmd != null;
        }

        static void Init_Vars()
        {
            cmd_ass_ = AddContract(new("var",
                args: static exe =>
                {
                    if (exe.reader.TryReadArgument(out string varname))
                        if (exe.reader.HasNext())
                            if (exe.reader.TryReadChar('='))
                                if (exe.harbinger.TryParseExpression(exe.reader, out var expression, out exe.error))
                                {
                                    Variable<object> variable = new(varname, null);
                                    exe.harbinger.global_variables[varname] = variable;
                                    exe.args.Add(variable);
                                    exe.args.Add(expression);
                                }
                },
                routine: static exe =>
                {
                    Variable<object> variable = (Variable<object>)exe.args[0];
                    Executor expression = (Executor)exe.args[1];
                    return expression.EExecute(data => variable.value = data);
                }));
        }
    }
}