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
                    if (exe.pipe_previous == null)
                        if (exe.harbinger.TryParseExpression(exe.reader, exe.parent, true, out var expr, out exe.error))
                            exe.args.Add(expr);
                },
                routine: static exe =>
                {
                    ExpressionExecutor expr = (ExpressionExecutor)exe.args[0];
                    return Executor.EExecute(
                        after_execution: expr.pipe_next == null ? exe.harbinger.stdout : null,
                        modify_output: null,
                        expr.EExecute());
                }));
        }
    }
}