using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init_Casts()
        {
            AddContract(new("int",
                min_args: 1,
                args: static exe =>
                {
                    if (exe.harbinger.TryParseExpression(exe.reader, exe.parent, true, out var expr, out exe.error))
                        exe.args.Add(expr);
                },
                routine: static exe =>
                {
                    ExpressionExecutor expr = (ExpressionExecutor)exe.args[0];
                    return Executor.EExecute(null, data => data switch
                    {
                        int i => i,
                        string s => int.Parse(s),
                        _ => 0,
                    },
                    expr.EExecute());
                }));

            AddContract(new("float",
                min_args: 1,
                args: static exe =>
                {
                    if (exe.harbinger.TryParseExpression(exe.reader, exe.parent, true, out var expr, out exe.error))
                        exe.args.Add(expr);
                },
                routine: static exe =>
                {
                    ExpressionExecutor expr = (ExpressionExecutor)exe.args[0];
                    return Executor.EExecute(null, data => data switch
                    {
                        int i => i,
                        float f => f,
                        string s => Util.ParseFloat(s),
                        _ => 0,
                    },
                    expr.EExecute());
                }));
        }
    }
}