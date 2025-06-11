using System;
using System.Collections.Generic;
using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        static Contract cmd_unary_;

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init_Unary()
        {
            cmd_unary_ = AddContract(new("unary",
                args: static exe =>
                {
                    if (!exe.reader.TryReadArgument(out string arg, out exe.error))
                        exe.error ??= "expected operator or factor";
                    else if (!Enum.TryParse(arg, true, out OperatorsM code))
                        exe.error = $"unknown operator '{arg}'";
                    else if (exe.harbinger.TryParseFactor(exe.reader, out var factor, out exe.error))
                    {
                        exe.args.Add(code);
                        exe.args.Add(factor);
                    }
                },
                routine: EUnary));

            static IEnumerator<Contract.Status> EUnary(ContractExecutor exe)
            {
                OperatorsM code = (OperatorsM)exe.args[0];
                Executor factor = (Executor)exe.args[1];

                var routine = factor.EExecute();
                while (routine.MoveNext())
                    yield return routine.Current;
                object data = routine.Current.data;

                yield return new Contract.Status()
                {
                    data = code switch
                    {
                        OperatorsM.not => !(bool)data,
                        OperatorsM.add => +(int)data,
                        OperatorsM.sub => -(int)data,
                        _ => data,
                    },
                };
            }
        }
    }
}