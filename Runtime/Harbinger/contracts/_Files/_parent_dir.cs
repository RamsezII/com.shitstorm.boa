using System.IO;
using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void InitCmd_ParentDir()
        {
            AddContract(new("parent_dir", typeof(string),
                min_args: 1,
                args: static exe =>
                {
                    if (exe.pipe_previous == null)
                        if (!exe.harbinger.TryParseExpression(exe.reader, exe.scope, true, typeof(string), out var expr))
                            exe.reader.Stderr($"expects expression of type path.");
                        else
                            exe.arg_0 = expr;
                },
                routine: static exe => Executor.EExecute(
                    after_execution: null,
                    modify_output: data =>
                    {
                        string path = (string)data;
                        string parent = Directory.GetParent(path).FullName;
                        return exe.harbinger.PathCheck(parent, PathModes.TryMaintain, true, false, out _, out _);
                    },
                    exe.arg_0.EExecute())
                ));
        }
    }
}