using System.Collections.Generic;
using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init_Sleep()
        {
            AddContract(new("sleep",
                min_args: 1,
                args: static exe =>
                {
                    if (exe.harbinger.TryParseExpression(exe.reader, true, out var expr, out exe.error))
                        exe.args.Add(expr);
                },
                routine: ESleep));

            static IEnumerator<Contract.Status> ESleep(ContractExecutor exe)
            {
                ContractExecutor expr = (ContractExecutor)exe.args[0];

                float time = 0;
                var routine = expr.EExecute(data => time = (float)data);
                while (routine.MoveNext())
                    yield return routine.Current;

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