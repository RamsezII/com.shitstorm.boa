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
                    if (exe.harbinger.TryParseExpression(exe.reader, exe, true, out var expr))
                        exe.args.Add(expr);
                },
                routine: EStdin));

            static IEnumerator<Contract.Status> EStdin(ContractExecutor exe)
            {
                ExpressionExecutor expr = (ExpressionExecutor)exe.args[0];

                var routine = expr.EExecute();
                while (routine.MoveNext())
                    yield return routine.Current;

                string prefixe = routine.Current.data.IterateThroughData_str().FirstOrDefault();

                Contract.Status status_last = new(Contract.Status.States.WAIT_FOR_STDIN, prefixe: prefixe);

                string stdin;
                while (!exe.harbinger.TryPullStdin(out stdin))
                    yield return status_last;

                yield return new(Contract.Status.States.ACTION_skip, data: stdin);
            }
        }
    }
}