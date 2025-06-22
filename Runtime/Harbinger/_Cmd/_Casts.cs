using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init_Casts()
        {
            AddContract(new("typeof",
                outputs_if_end_of_instruction: true,
                args: static exe =>
                {
                    if (exe.pipe_previous == null && exe.harbinger.TryParseExpression(exe.reader, exe.scope, true, out var expr))
                        exe.arg_0 = expr;
                },
                routine: static exe => Executor.EExecute(
                    after_execution: null,
                    modify_output: static data => data?.GetType(),
                    exe.arg_0.EExecute()
                    )
                ));

            AddContract(new("int",
                min_args: 1,
                args: static exe =>
                {
                    if (exe.pipe_previous == null && exe.harbinger.TryParseExpression(exe.reader, exe.scope, true, out var expr))
                        exe.arg_0 = expr;
                },
                routine: static exe => Executor.EExecute(
                    after_execution: null,
                    modify_output: data => data switch
                    {
                        int i => i,
                        float f => (int)f,
                        string s => int.Parse(s),
                        _ => 0,
                    },
                    exe.arg_0.EExecute()
                    )
                ));

            AddContract(new("float",
                min_args: 1,
                args: static exe =>
                {
                    if (exe.pipe_previous == null && exe.harbinger.TryParseExpression(exe.reader, exe.scope, true, out var expr))
                        exe.arg_0 = expr;
                },
                routine: static exe => Executor.EExecute(null, data => data switch
                    {
                        int i => i,
                        float f => f,
                        string s => Util.ParseFloat(s),
                        _ => 0,
                    },
                    exe.arg_0.EExecute())
                ));
        }
    }
}