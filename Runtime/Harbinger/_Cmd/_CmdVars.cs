using System;

namespace _BOA_
{
    partial class Harbinger
    {
        enum OperatorsE : byte
        {
            assign,
            add, sub,
            mul, div, div_int, mod,
            eq, neq, gt, lt,
            and, or, xor,
        }

        [Flags]
        public enum OperatorsM : ushort
        {
            unknown,
            assign = 1 << OperatorsE.assign,
            add = 1 << OperatorsE.add,
            sub = 1 << OperatorsE.sub,
            mul = 1 << OperatorsE.mul,
            div = 1 << OperatorsE.div,
            div_int = 1 << OperatorsE.div_int,
            mod = 1 << OperatorsE.mod,
            eq = 1 << OperatorsE.eq,
            neq = 1 << OperatorsE.neq,
            gt = 1 << OperatorsE.gt,
            lt = 1 << OperatorsE.lt,
            ge = gt | eq,
            le = lt | eq,
            and = 1 << OperatorsE.and,
            or = 1 << OperatorsE.or,
            xor = 1 << OperatorsE.xor,
        }

        static Contract
            cmd_literal = new("literal", typeof(object), action: static exe => exe.args[0]),
            cmd_variable = new("variable", typeof(object), action: static exe => ((Variable<object>)exe.args[0]).value),
            cmd_declare_,
            cmd_assign_,
            cmd_math_,
            cmd_not_;

        //----------------------------------------------------------------------------------------------------------

        static void Init_Vars()
        {
            cmd_declare_ = AddContract(new("var",
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

            cmd_assign_ = AddContract(new("assign",
                args: static exe =>
                {
                    if (exe.reader.TryReadArgument(out string varname))
                        if (exe.reader.TryReadArgument(out string operator_name))
                            if (!Enum.TryParse(operator_name, true, out OperatorsM code))
                                exe.error = $"unknown operator '{operator_name}'";
                            else if (exe.harbinger.TryParseExpression(exe.reader, out var expression, out exe.error))
                            {
                                Variable<object> variable = new(varname, null);
                                exe.harbinger.global_variables[varname] = variable;
                                exe.args.Add(code);
                                exe.args.Add(variable);
                                exe.args.Add(expression);
                            }
                },
                routine: static exe =>
                {
                    OperatorsM code = (OperatorsM)exe.args[0];
                    Variable<object> variable = (Variable<object>)exe.args[1];
                    Executor expression = (Executor)exe.args[2];
                    return expression.EExecute(data => variable.value = (code & ~OperatorsM.assign) switch
                    {
                        OperatorsM.add => (int)variable.value + (int)data,
                        OperatorsM.sub => (int)variable.value - (int)data,
                        OperatorsM.mul => (int)variable.value * (int)data,
                        OperatorsM.div => (int)variable.value / (int)data,
                        OperatorsM.div_int => (int)variable.value / (int)data,
                        OperatorsM.mod => (int)variable.value % (int)data,
                        _ => data,
                    });
                }));

            cmd_math_ = AddContract(new("math",
                args: static exe =>
                {
                    if (exe.reader.TryReadArgument(out string operator_name))
                        if (!Enum.TryParse(operator_name, true, out OperatorsM code))
                            exe.error = $"unknown operator '{operator_name}'";
                        else if (exe.harbinger.TryParseExpression(exe.reader, out var expr1, out exe.error))
                            if (exe.harbinger.TryParseExpression(exe.reader, out var expr2, out exe.error))
                            {
                                exe.args.Add(code);
                                exe.args.Add(expr1);
                                exe.args.Add(expr2);
                            }
                },
                routine: static exe =>
                {
                    OperatorsM code = (OperatorsM)exe.args[0];
                    Executor expr1 = (Executor)exe.args[1];
                    Executor expr2 = (Executor)exe.args[2];

                    object data1 = null, data2 = null;

                    return Executor.EExecute(
                        () =>
                        {
                            if (data1 is int i1 && data2 is int i2)
                                return code switch
                                {
                                    OperatorsM.add => i1 + i2,
                                    OperatorsM.sub => i1 - i2,
                                    OperatorsM.mul => i1 * i2,
                                    OperatorsM.div => i1 / i2,
                                    OperatorsM.div_int => i1 / i2,
                                    OperatorsM.mod => i1 % i2,
                                    OperatorsM.eq => i1 == i2,
                                    OperatorsM.neq => i1 != i2,
                                    OperatorsM.gt => i1 > i2,
                                    OperatorsM.lt => i1 < i2,
                                    OperatorsM.ge => i1 >= i2,
                                    OperatorsM.le => i1 <= i2,
                                    OperatorsM.and => i1 & i2,
                                    OperatorsM.or => i1 | i2,
                                    OperatorsM.xor => i1 ^ i2,
                                    _ => 0,
                                };
                            if (data1 is bool b1 && data2 is bool b2)
                                return code switch
                                {
                                    OperatorsM.and => b1 & b2,
                                    OperatorsM.or => b1 | b2,
                                    OperatorsM.xor => b1 ^ b2,
                                    _ => false,
                                };
                            return 0;
                        },
                        (expr1, data => data1 = data),
                        (expr2, data => data2 = data)
                        );
                }));
        }
    }
}