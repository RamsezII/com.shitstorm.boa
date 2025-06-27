using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init_Stdin()
        {
            AddContract(new("stdin", typeof(string),
                max_args: 1,
                args: static exe =>
                {
                    if (exe.pipe_previous == null)
                        if (exe.harbinger.TryParseExpression(exe.reader, exe.scope, false, typeof(string), out var expr))
                            exe.arg_0 = expr;
                },
                routine: EStdin));

            static IEnumerator<Contract.Status> EStdin(ContractExecutor exe)
            {
                string prefixe = string.Empty;

                if (exe.arg_0 != null)
                {
                    var routine = exe.arg_0.EExecute();
                    while (routine.MoveNext())
                        yield return routine.Current;

                    prefixe = routine.Current.output.IterateThroughData_str().FirstOrDefault();
                }

                Contract.Status status_last = new(Contract.Status.States.WAIT_FOR_STDIN, prefixe_text: prefixe);

                do
                    yield return status_last;
                while (!exe.harbinger.signal.flags.HasFlag(SIG_FLAGS_new.SUBMIT));

                string stdin = exe.harbinger.signal.reader.ReadAll();
                yield return new(Contract.Status.States.ACTION_skip, output: stdin);
            }
        }
    }
}