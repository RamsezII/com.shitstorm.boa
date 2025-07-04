﻿using System.Collections.Generic;
using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void InitCmd_Waiters()
        {
            AddContract(new("wait_one_frame",
                output_type: null,
                routine: EWaitOneFrame
                ));

            static IEnumerator<Contract.Status> EWaitOneFrame(ContractExecutor executor)
            {
                while (!executor.harbinger.signal.flags.HasFlag(SIG_FLAGS_new.TICK))
                    yield return default;
                yield return default;
            }

            AddContract(new("sleep",
                output_type: null,
                min_args: 1,
                args: static exe =>
                {
                    if (exe.pipe_previous == null && exe.harbinger.TryParseExpression(exe.reader, exe.scope, true, typeof(float), out var expr))
                        exe.arg_0 = expr;
                },
                routine: ESleep));

            static IEnumerator<Contract.Status> ESleep(ContractExecutor exe)
            {
                var routine = exe.arg_0.EExecute();
                while (routine.MoveNext())
                    yield return routine.Current;

                switch (routine.Current.output)
                {
                    case float or int:
                        break;
                    default:
                        exe.harbinger.Stderr($"can not use '{routine.Current.output}' as timer value (expected {typeof(int)} or {typeof(float)}, got {routine.Current.output?.GetType()})");
                        yield break;
                }

                float time = routine.Current.output switch
                {
                    int i => i,
                    float f => f,
                    _ => 0,
                };

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