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
            Init_For();

            AddContract(new(
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
                }));

            AddContract(new(
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