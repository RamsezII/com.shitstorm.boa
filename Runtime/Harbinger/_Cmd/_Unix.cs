using System.IO;
using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void InitCmd_Unix()
        {
            AddContract(new("cd",
                min_args: 1,
                args: static exe =>
                {
                    if (exe.harbinger.TryParsePath(exe.reader, exe.scope, out string path))
                        exe.arg_0 = new LiteralExecutor(exe.harbinger, exe.scope, path);
                    else
                        exe.reader.sig_error ??= $"expected path string.";
                },
                routine: static exe =>
                {
                    return Executor.EExecute(
                        after_execution: data =>
                        {
                            string path = data is string str ? str : data.ToString();
                            path = exe.harbinger.shell.PathCheck(path, PathModes.ForceFull);

                            if (Directory.Exists(path))
                                exe.harbinger.shell.ChangeWorkdir(path);
                            else
                                exe.harbinger.Stderr($"Found no directory at path '{path}'.");
                        },
                        modify_output: null,
                        exe.arg_0.EExecute()
                    );
                }));
        }
    }
}