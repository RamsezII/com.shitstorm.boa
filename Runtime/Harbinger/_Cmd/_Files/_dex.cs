using System.IO;
using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void InitCmd_FS_Exists()
        {
            AddContract(new("directory-exists", typeof(bool),
                min_args: 1,
                outputs_if_end_of_instruction: true,
                args: static exe =>
                {
                    if (exe.pipe_previous == null)
                        if (exe.harbinger.TryParseExpression(exe.reader, exe.scope, true, out var expr))
                            exe.arg_0 = expr;
                        else
                            exe.reader.Stderr($"expected path expression.");
                },
                routine: static exe =>
                {
                    return Executor.EExecute(
                        after_execution: null,
                        modify_output: data =>
                        {
                            string path = (string)data;
                            path = exe.harbinger.PathCheck(path, PathModes.ForceFull, false, false, out _, out _);
                            return Directory.Exists(path);
                        },
                        exe.arg_0.EExecute());
                }),
                "dex");

            AddContract(new("file-exists", typeof(bool),
                min_args: 1,
                outputs_if_end_of_instruction: true,
                args: static exe =>
                {
                    if (exe.pipe_previous == null)
                        if (exe.harbinger.TryParseExpression(exe.reader, exe.scope, true, out var expr))
                            exe.arg_0 = expr;
                        else
                            exe.reader.Stderr($"expected path expression.");
                },
                routine: static exe =>
                {
                    return Executor.EExecute(
                        after_execution: null,
                        modify_output: data =>
                        {
                            string path = (string)data;
                            path = exe.harbinger.PathCheck(path, PathModes.ForceFull, false, false, out _, out _);
                            return File.Exists(path);
                        },
                        exe.arg_0.EExecute());
                }),
                "fex");
        }
    }
}