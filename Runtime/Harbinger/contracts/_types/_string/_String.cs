using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        static partial class CmdString
        {
            [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
            static void Init_String()
            {
                AddContract(new("str", typeof(string),
                    min_args: 1,
                    args: static exe =>
                    {
                        if (exe.pipe_previous == null)
                            if (!exe.harbinger.TryParseExpression(exe.reader, exe.scope, true, typeof(object), out var expr))
                                exe.reader.Stderr($"'{exe.contract.name}' expects an expression.");
                            else
                                exe.arg_0 = expr;
                    },
                    routine: static exe => Executor.EExecute(
                        modify_output: static data => data?.ToBoaString(),
                        stack: exe.arg_0.EExecute()
                        )
                    ));
            }
        }
    }
}