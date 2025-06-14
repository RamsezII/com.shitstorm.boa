using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void _Cmd_Harbinger()
        {
            AddContract(new("harbinger"));
        }
    }
}