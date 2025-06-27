using System;
using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init_Casts()
        {
            AddContract(new("typeof", typeof(Type),
                outputs_if_end_of_instruction: true,
                args: static exe =>
                {
                    if (exe.pipe_previous == null && exe.harbinger.TryParseExpression(exe.reader, exe.scope, true, typeof(object), out var expr))
                        exe.arg_0 = expr;
                },
                function: static exe => exe.arg_0.OutputType()
                ));

            AddSubContract(
                typeof(object),
                new("type", typeof(object), typeof(Type),
                    outputs_if_end_of_instruction: true,
                    function: static exe => ((SubContractExecutor)exe).output_exe.OutputType()
                    )
                );

            AddContract(new("bool", typeof(bool),
                min_args: 1,
                args: static exe =>
                {
                    if (exe.pipe_previous == null && exe.harbinger.TryParseExpression(exe.reader, exe.scope, true, typeof(object), out var expr))
                        exe.arg_0 = expr;
                },
                routine: static exe => Executor.EExecute(
                    after_execution: null,
                    modify_output: data => data.ToBool(),
                    exe.arg_0.EExecute()
                    )
                ));

            AddContract(new("int", typeof(int),
                min_args: 1,
                args: static exe =>
                {
                    if (exe.pipe_previous == null && exe.harbinger.TryParseExpression(exe.reader, exe.scope, true, typeof(object), out var expr))
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

            AddContract(new("float", typeof(float),
                min_args: 1,
                args: static exe =>
                {
                    if (exe.pipe_previous == null && exe.harbinger.TryParseExpression(exe.reader, exe.scope, true, typeof(object), out var expr))
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