using System.Collections.Generic;
using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        partial class CmdString
        {
            [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
            static void Init_String_others()
            {
                AddSubContract(new("mirrored", typeof(string), typeof(string),
                    routine: static exe => Executor.EExecute(
                        modify_output: static data => ((string)data).Mirror(),
                        stack: ((SubContractExecutor)exe).output_exe.EExecute()
                        )
                    ));

                AddSubContract(new("split",
                    input_type: typeof(string),
                    output_type: typeof(List<string>),
                    routine: static exe => Executor.EExecute(
                        after_execution: null,
                        modify_output: static data => ((string)data).Split(),
                        ((SubContractExecutor)exe).output_exe.EExecute()
                        )
                    ));
            }
        }
    }
}