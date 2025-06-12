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
                    if (exe.harbinger.TryParseExpression(exe.reader, true, out var expression, out exe.error))
                        exe.args.Add(expression);
                },
                routine: static exe =>
                {
                    Executor expression = (Executor)exe.args[0];
                    return expression.EExecute(after_execution: data =>
                    {
                        if (exe.pipe_next == null)
                            exe.harbinger.stdout(data);
                        else
                            exe.SendIntoPipe(data);
                    });
                }));
        }
    }
}