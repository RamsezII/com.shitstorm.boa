using System.Collections.Generic;
using UnityEngine;

namespace _BOA_
{
    static internal partial class Contracts
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            Harbinger.AddContract(new(
                "print",
                typeof(object),
                min_args: 1,
                args: static cont =>
                {
                    if (cont.reader.TryReadArgument(out string arg))
                        cont.args.Add(arg);
                },
                action: static cont =>
                {
                    cont.stdout(cont.args[0]);
                })
                );

            Harbinger.AddContract(new(
                "wait",
                min_args: 1,
                args: static cont =>
                {
                    if (cont.reader.TryReadArgument(out string arg))
                        cont.args.Add(arg);
                },
                routine: EWait)
                );

            static IEnumerator<Contract.Status> EWait(Contractor contractor)
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