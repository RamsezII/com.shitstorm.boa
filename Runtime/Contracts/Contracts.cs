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
                min_args: 1,
                args: static cont =>
                {
                    if (cont.TryReadArgument(out string arg))
                        cont.args.Add(arg);
                },
                action: static cont =>
                {
                    return new() { data = cont.args[0], };
                })
                );

            Harbinger.AddContract(new(
                "wait",
                min_args: 1,
                args: static cont =>
                {

                },
                routine: EWait)
                );

            static IEnumerator<Harbinger.Contract.Status> EWait(Harbinger.Contractor contractor)
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