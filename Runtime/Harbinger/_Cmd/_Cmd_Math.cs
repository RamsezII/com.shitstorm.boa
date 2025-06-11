using System;
using System.Collections.Generic;
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
            cmd_math_ = AddContract(new("math",
                args: static exe =>
                {
                    if (exe.reader.TryReadArgument(out string operator_name, out exe.error))
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
                routine: EMath));

            static IEnumerator<Contract.Status> EMath(ContractExecutor exe)
            {
                OperatorsM code = (OperatorsM)exe.args[0];
                Executor expr1 = (Executor)exe.args[1];
                Executor expr2 = (Executor)exe.args[2];

                var routine1 = expr1.EExecute();
                while (routine1.MoveNext())
                    yield return routine1.Current;
                object data1 = routine1.Current.data;

                var routine2 = expr2.EExecute();
                while (routine2.MoveNext())
                    yield return routine2.Current;
                object data2 = routine2.Current.data;

                if (data1 is int i1 && data2 is int i2)
                    yield return new Contract.Status()
                    {
                        data = code switch
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
                        },
                    };
                else if (data1 is bool b1 && data2 is bool b2)
                    yield return new Contract.Status()
                    {
                        data = code switch
                        {
                            OperatorsM.and => b1 & b2,
                            OperatorsM.or => b1 | b2,
                            OperatorsM.xor => b1 ^ b2,
                            _ => false,
                        },
                    };
                else
                    yield return default;
            }
        }
    }
}