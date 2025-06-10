using System.Collections.Generic;
using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        static void InitContracts()
        {
            Init_Vars();
            Init_If();
            Init_Cmd();
            Init_Stdin();

            AddContract(new(
                "echo",
                typeof(object),
                min_args: 1,
                args: static exe =>
                {
                    if (exe.harbinger.TryParseExpression(exe.reader, out var statement, out exe.error))
                        exe.args.Add(statement);
                },
                routine: static exe =>
                {
                    Executor statement = (Executor)exe.args[0];
                    return statement.EExecute(exe.harbinger.stdout);
                }));

            AddContract(new(
                "sleep",
                min_args: 1,
                args: static exe =>
                {
                    if (exe.reader.TryReadArgument(out string arg))
                        if (Util.TryParseFloat(arg, out float time))
                            exe.args.Add(time);
                },
                routine: EWait)
                );

            static IEnumerator<Contract.Status> EWait(ContractExecutor exe)
            {
                float time = (float)exe.args[0];
                float timer = 0;

                while (timer < time)
                {
                    timer += Time.unscaledDeltaTime;
                    if (timer < time)
                        yield return new() { progress = timer / time, };
                    else
                        yield return new() { progress = 1, };
                }
            }
        }
    }
}