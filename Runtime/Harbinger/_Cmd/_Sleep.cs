using System.Collections.Generic;
using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init_Sleep()
        {
            AddContract(new(
                "sleep",
                min_args: 1,
                args: static exe =>
                {
                    if (exe.reader.TryReadArgument(out string arg))
                        if (Util.TryParseFloat(arg, out float time))
                            exe.args.Add(time);
                },
                routine: ESleep)
                );

            static IEnumerator<Contract.Status> ESleep(ContractExecutor exe)
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