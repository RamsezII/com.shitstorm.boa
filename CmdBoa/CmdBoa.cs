using _COBRA_;
using UnityEngine;

namespace _BOA_
{
    static internal class CmdBoa
    {
        static readonly Command instance = new(manual: new("write your own shitstorm scripts :)"));

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            Command.cmd_root_shell.AddCommand("boa", instance);

            instance.AddCommand("execute-script", new(
                manual: new("execute script at path <path>"),
                args: exe =>
                {
                    if (exe.line.TryReadArgument(out string arg))
                        exe.args.Add(arg);
                },
                action: exe =>
                {
                    exe.Stdout("Executing script at path: " + exe.args[0]);
                }),
                "execute");
        }
    }
}