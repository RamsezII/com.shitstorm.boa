using UnityEngine;

namespace _BOA_
{
    partial class Harbinger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void InitCmd_Unix()
        {
            AddContract(new("cd",
            min_args: 1,
            action: static exe =>
            {

            }));
        }
    }
}