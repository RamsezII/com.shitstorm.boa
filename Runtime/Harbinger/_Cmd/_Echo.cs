using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init_Echo()
        {
            AddContract(new(
                "echo",
                typeof(object),
                min_args: 1,
                args: static exe =>
                {
                    if (exe.harbinger.TryParseExpression(exe.reader, out var statement, out exe.error))
                        exe.args.Add(statement);
                },
                routine: static exe =>
                {
                    Executor statement = (Executor)exe.args[0];
                    return statement.EExecute(exe.harbinger.stdout);
                }));
        }
    }
}