using System;
using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        static Contract cmd_math_;

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init_Math()
        {
            AddContract(cmd_math_ = new("math", typeof(object),
                outputs_if_end_of_instruction: true,
                args: static exe =>
                {
                    if (exe.reader.TryReadArgument(out string operator_name, true, lint: exe.reader.lint_theme.operators))
                        if (!Enum.TryParse(operator_name, true, out OperatorsM code))
                            exe.reader.Stderr($"unknown operator '{operator_name}'.");
                        else if (exe.harbinger.TryParseExpression(exe.reader, exe.scope, true, typeof(object), out var expr1))
                            if (exe.harbinger.TryParseExpression(exe.reader, exe.scope, true, typeof(object), out var expr2))
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
                    var routine1 = expr1.EExecute();
                    var routine2 = expr2.EExecute();

                    return Executor.EExecute(
                        after_execution: null,
                        modify_output: data =>
                        {
                            object data1 = routine1.Current.output;
                            object data2 = routine2.Current.output;

                            if (data1 is int iballs && data2 is Vector3 vballs)
                                return iballs * vballs;

                            if (data1 is int int1 && data2 is int int2)
                                return code switch
                                {
                                    OperatorsM.add => int1 + int2,
                                    OperatorsM.sub => int1 - int2,
                                    OperatorsM.mul => int1 * int2,
                                    OperatorsM.div or OperatorsM.div_int => int1 / int2,
                                    OperatorsM.mod => int1 % int2,
                                    OperatorsM.eq => int1 == int2,
                                    OperatorsM.neq => int1 != int2,
                                    OperatorsM.gt => int1 > int2,
                                    OperatorsM.lt => int1 < int2,
                                    OperatorsM.ge => int1 >= int2,
                                    OperatorsM.le => int1 <= int2,
                                    OperatorsM.and => int1 & int2,
                                    OperatorsM.or => int1 | int2,
                                    OperatorsM.xor => int1 ^ int2,
                                    _ => 0,
                                };

                            if (data1 is float f1 && data2 is float f2)
                                return code switch
                                {
                                    OperatorsM.add => f1 + f2,
                                    OperatorsM.sub => f1 - f2,
                                    OperatorsM.mul => f1 * f2,
                                    OperatorsM.div or OperatorsM.div_int => f1 / f2,
                                    OperatorsM.mod => f1 % f2,
                                    OperatorsM.eq => f1 == f2,
                                    OperatorsM.neq => f1 != f2,
                                    OperatorsM.gt => f1 > f2,
                                    OperatorsM.lt => f1 < f2,
                                    OperatorsM.ge => f1 >= f2,
                                    OperatorsM.le => f1 <= f2,
                                    _ => 0,
                                };

                            if (data1 is bool b1 && data2 is bool b2)
                                return code switch
                                {
                                    OperatorsM.eq => b1 == b2,
                                    OperatorsM.and => b1 & b2,
                                    OperatorsM.or => b1 | b2,
                                    OperatorsM.xor => b1 ^ b2,
                                    _ => false,
                                };

                            if (data1 is string || data2 is string)
                            {
                                if (data1 is not string str1)
                                    str1 = data1?.ToString() ?? string.Empty;

                                if (data2 is not string str2)
                                    str2 = data2?.ToString() ?? string.Empty;

                                switch (code)
                                {
                                    case OperatorsM.add:
                                        return $"{str1}{str2}";

                                    case OperatorsM.eq:
                                        return str1.Equals(str2, StringComparison.Ordinal);

                                    case OperatorsM.neq:
                                        return !str1.Equals(str2, StringComparison.Ordinal);
                                }
                            }

                            if (code == OperatorsM.eq)
                                if (data1 != null && data2 != null)
                                    return data1.Equals(data2);

                            exe.harbinger.Stderr($"could not apply operation '{code}' on {data1} ({data1?.GetType()}) and {data2} ({data2?.GetType()})");
                            return null;
                        },
                        routine1,
                        routine2
                    );
                }));
        }
    }
}