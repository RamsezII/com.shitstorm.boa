using _UTIL_;
using System.IO;
using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void InitCmd_Unix_cat()
        {
            AddContract(new("cat",
                outputs_if_end_of_instruction: true,
                args: static exe =>
                {
                    if (exe.harbinger.TryParsePath(exe.reader, FS_TYPES.FILE, true, out string path))
                    {
                        path = exe.harbinger.shell.PathCheck(path, PathModes.ForceFull, false, false, out _, out _);
                        exe.arg_0 = new LiteralExecutor(exe.harbinger, exe.scope, path);
                    }
                },
                routine: static exe =>
                {
                    return Executor.EExecute(
                        after_execution: null,
                        modify_output: data =>
                        {
                            string path = data is string str ? str : data.ToString();
                            if (File.Exists(path))
                            {
                                string cat = File.ReadAllText(path).Trim();
                                return cat;
                            }
                            exe.harbinger.Stderr($"Could not find file '{path}'.");
                            return null;
                        },
                        exe.arg_0.EExecute());
                }));
        }
    }
}