using _COBRA_;
using System.Collections.Generic;
using UnityEngine;

namespace _BOA_
{
    static internal class CmdBoa
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            Command cmd_boa = Shell.static_domain.AddDomain(
                "boa",
                manual: new("write your own shitstorm scripts :)")
                );

            cmd_boa.AddRoutine(
                "execute-script",
                manual: new("execute script at path <path>"),
                min_args: 1,
                args: exe =>
                {
                    if (exe.line.TryReadArgument(out string arg))
                        exe.args.Add(arg);
                },
                routine: ERoutine);

            static IEnumerator<CMD_STATUS> ERoutine(Command.Executor exe)
            {
                yield break;
            }
        }
    }
}