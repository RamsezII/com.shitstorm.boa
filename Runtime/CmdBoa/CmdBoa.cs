using _COBRA_;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace _BOA_
{
    static internal class CmdBoa
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            Command domain_boa = Command.static_domain.AddDomain(
                "boa",
                manual: new("create and run your own shitstorm scripts :)")
                );

            domain_boa.AddRoutine(
                "run",
                manual: new("execute script at path <path>"),
                min_args: 1,
                args: exe =>
                {
                    if (exe.line.TryReadArgument(out string script_path, out _, strict: true, path_mode: PATH_FLAGS.FILE))
                        exe.args.Add(script_path);
                },
                routine: ERun);

            static IEnumerator<CMD_STATUS> ERun(Command.Executor exe)
            {
                string script_path = (string)exe.args[0];
                script_path = exe.shell.PathCheck(script_path, PathModes.ForceFull);

                string script = File.ReadAllText(script_path);

                while (true)
                {
                    int read_i = 0;
                    foreach (string script_line in script.IterateThroughData_str())
                    {
                        Command.Line cmd_line = new(script_line, exe.line.signal, exe.shell, cursor_i: int.MaxValue);
                        exe.shell.PropagateLine(cmd_line);
                        read_i += script_line.Length;
                    }
                    yield return new CMD_STATUS(CMD_STATES.BLOCKING);
                }
            }
        }
    }
}