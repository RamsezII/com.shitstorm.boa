using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void InitCmd_List_tolist()
        {
            AddContract(new("to_list",
                output_type: typeof(List<object>),
                min_args: 1,
                args: static exe =>
                {
                    if (exe.pipe_previous == null)
                        if (exe.harbinger.TryParseExpression(exe.reader, exe.scope, true, typeof(object), out var expr))
                            exe.arg_0 = expr;
                        else
                            exe.reader.Stderr("expected expression.");
                },
                routine: static exe => Executor.EExecute(
                    after_execution: null,
                    modify_output: static data => data?.IterateThroughData().ToList(),
                    exe.arg_0.EExecute()
                    )
                ));

            AddSubContract(new("to_list", typeof(object), typeof(List<object>),
                routine: static exe => Executor.EExecute(
                    after_execution: null,
                    modify_output: static data => data?.IterateThroughData().ToList(),
                    ((SubContractExecutor)exe).output_exe.EExecute()
                    )
                ));
        }
    }
}