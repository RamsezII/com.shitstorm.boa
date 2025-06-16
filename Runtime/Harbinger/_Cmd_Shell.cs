using _COBRA_;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void InitCmd_Shell()
        {
            Command.static_domain.AddRoutine("harbinger", routine: ERoutine);

            static IEnumerator<CMD_STATUS> ERoutine(Command.Executor cexe)
            {
                string prefixe = ">";

                CMD_STATUS shell_status = new(CMD_STATES.WAIT_FOR_STDIN, prefixe: prefixe);

                while (true)
                {
                    if (!string.IsNullOrWhiteSpace(cexe.line.text))
                        if (cexe.line.TryReadArgument(out string arg, out _, completions: global_contracts.Keys))
                            if (cexe.line.flags.HasFlag(SIG_FLAGS.SUBMIT))
                                Debug.Log("cmd: " + arg);
                    yield return shell_status;
                }
            }
        }
    }
}