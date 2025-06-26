using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void InitCmd__Random()
        {
            AddContract(new("rand01",
                function: static exe => Random.Range(0f, 1f)
                ));

            AddContract(new("randint",
                min_args: 2,
                args: static exe =>
                {
                    if (!exe.harbinger.TryParseExpression(exe.reader, exe.scope, true, out var expr_min))
                        exe.reader.Stderr($"expected int expr (min).");
                    else if (!exe.harbinger.TryParseExpression(exe.reader, exe.scope, true, out var expr_max))
                        exe.reader.Stderr($"expected int expr (max).");
                    else
                    {
                        exe.args.Add(expr_min);
                        exe.args.Add(expr_max);
                    }
                },
                routine: static exe =>
                {
                    ExpressionExecutor expr_min = (ExpressionExecutor)exe.args[0];
                    ExpressionExecutor expr_max = (ExpressionExecutor)exe.args[1];

                    using var routine_min = expr_min.EExecute();
                    using var routine_max = expr_max.EExecute();

                    return Executor.EExecute(
                        after_execution: null,
                        modify_output: _ =>
                        {
                            int min = (int)routine_min.Current.output;
                            int max = (int)routine_max.Current.output;
                            return Random.Range(min, max);
                        },
                        routine_min,
                        routine_max);
                }
                ));
        }
    }
}