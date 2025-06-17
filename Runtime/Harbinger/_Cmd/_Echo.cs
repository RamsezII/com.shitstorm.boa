using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init_Echo()
        {
            AddContract(new("echo",
                min_args: 1,
                args: static exe =>
                {
                    if (exe.pipe_previous == null && exe.harbinger.TryParseExpression(exe.reader, exe.scope, true, out var expr))
                        exe.arg_0 = expr;
                },
                routine: static exe =>
                {
                    if (exe.pipe_next == null)
                        return Executor.EExecute(
                            after_execution: exe.harbinger.stdout,
                            modify_output: null,
                            exe.arg_0.EExecute());
                    else
                        return exe.arg_0.EExecute();
                }));
        }
    }
}