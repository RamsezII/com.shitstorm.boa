using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init_Stdin()
        {
            AddContract(new("stdin",
                min_args: 1,
                args: static exe =>
                {
                    if (exe.harbinger.TryParseExpression(exe.reader, true, out var expr, out exe.error))
                        exe.args.Add(expr);
                },
                routine: EStdin));

            static IEnumerator<Contract.Status> EStdin(ContractExecutor exe)
            {
                ContractExecutor expr = (ContractExecutor)exe.args[0];

                var routine = expr.EExecute();
                while (routine.MoveNext())
                    yield return routine.Current;

                string prefixe = routine.Current.data.IterateThroughData_str().FirstOrDefault();

                Contract.Status status_last = new()
                {
                    state = Contract.Status.States.WAIT_FOR_STDIN,
                    prefixe = prefixe,
                };

                string stdin;
                while (!Util.TryPullValue(ref exe.harbinger.shell_stdin, out stdin))
                    yield return status_last;

                yield return new()
                {
                    state = Contract.Status.States.ACTION_skip,
                    data = stdin,
                };
            }
        }
    }
}