using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void InitCmd_Harbinger()
        {
            AddContract(new("harbinger"));
        }
    }
}