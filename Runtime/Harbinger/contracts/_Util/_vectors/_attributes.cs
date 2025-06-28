using System.Collections.Generic;
using UnityEngine;

namespace _BOA_
{
    static partial class BoaVector
    {
        class SubContract_xyz : SubContract
        {
            readonly int index;

            //----------------------------------------------------------------------------------------------------------

            public SubContract_xyz(
                in string name
                ) : base(
                    name,
                    object_type: typeof(Vector3),
                    attribute_type: typeof(float),
                    function_style_arguments: false,
                    no_semicolon_required: true,
                    no_parenthesis: true,
                    outputs_if_end_of_instruction: true,
                    routine: XYZ_ERoutine)
            {
                index = name[0] switch
                {
                    'x' => 0,
                    'y' => 1,
                    'z' => 2,
                    'w' => 3,
                    _ => -1,
                };
            }

            //----------------------------------------------------------------------------------------------------------

            static IEnumerator<Status> XYZ_ERoutine(ContractExecutor exe) => Executor.EExecute(
                modify_output: data =>
                {
                    Vector3 output = (Vector3)data;
                    SubContract_xyz contract = (SubContract_xyz)exe.contract;
                    return output[contract.index];
                },
                stack: ((SubContractExecutor)exe).output_exe.EExecute()
                );
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void InitCmd_Vectors_attributes()
        {
            Harbinger.AddSubContract(new SubContract_xyz("x"));
            Harbinger.AddSubContract(new SubContract_xyz("y"));
            Harbinger.AddSubContract(new SubContract_xyz("z"));
            Harbinger.AddSubContract(new SubContract_xyz("w"));
        }
    }
}