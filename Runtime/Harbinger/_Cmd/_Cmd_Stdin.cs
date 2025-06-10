using System.Collections.Generic;
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
                    if (exe.reader.TryReadArgument(out string arg))
                        exe.args.Add(arg);
                },
                routine: EStdin));

            static IEnumerator<Contract.Status> EStdin(ContractExecutor exe)
            {
                Contract.Status status = new()
                {
                    state = Contract.Status.States.WAIT_FOR_STDIN,
                    prefixe = (string)exe.args[0],
                };

                while (true)
                    yield return status;
            }
        }
    }
}