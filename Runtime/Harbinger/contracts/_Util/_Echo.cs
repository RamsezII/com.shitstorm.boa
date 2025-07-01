using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init_Echo()
        {
            AddContract(new("echo", typeof(object),
                get_output_type: static exe => exe.arg_0.OutputType(),
                min_args: 1,
                outputs_if_end_of_instruction: true,
                args: static exe =>
                {
                    if (exe.pipe_previous == null)
                        if (exe.harbinger.TryParseExpression(exe.reader, exe.scope, true, typeof(object), out var expr))
                            exe.arg_0 = expr;
                        else
                            exe.reader.Stderr($"'echo' expected expression.");
                },
                routine: static exe =>  exe.arg_0.EExecute()
                ));
        }
    }
}