using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void InitCmd_Files_other()
        {
            AddContract(new("_workdir_", null, function: static exe => exe.harbinger.workdir));
        }
    }
}