using System.Collections.Generic;
using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        static void InitContracts()
        {
            Init_Vars();

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
                args: static cont =>
                {
                    if (cont.reader.TryReadArgument(out string arg))
                        cont.args.Add(arg);
                },
                routine: EWait)
                );

            static IEnumerator<Contract.Status> EWait(ContractExecutor contractor)
            {
                float timer = 0;

                while (timer < 1)
                {
                    timer += Time.unscaledDeltaTime;
                    yield return new() { progress = timer, };
                }
            }
        }
    }
}